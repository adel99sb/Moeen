using Microsoft.EntityFrameworkCore;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Core.Contracts.infrastructure.Providers;
using Moeen.Api.Core.Entities;
using Moeen.Api.infrastructure.Data;
using Moeen.Shared.Constants;
using Moeen.Shared.Requests.Goal;
using Moeen.Shared.Responses;
using Moeen.Shared.Responses.Goal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Moeen.Api.Application.Services
{
    public class GoalService : IGoalService
    {
        private readonly AppDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public GoalService(AppDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<GeneralResponse> RecordDailyEntryAsync(RecordDailyEntryRequest request)
        {
            if (request == null || request.StudentId == Guid.Empty)
                return GeneralResponse.BadRequest("بيانات الطالب مطلوبة.");

            var teacherId = _currentUserService.CurrentUserId;
            if (!teacherId.HasValue)
                return GeneralResponse.Unauthorized("غير مصرح.");

            var student = await _context.Students.FirstOrDefaultAsync(s => s.Id == request.StudentId);
            if (student == null)
                return GeneralResponse.NotFound("الطالب غير موجود.");

            var halqaId = await ResolveHalqaIdAsync(teacherId.Value, student.SaturdayHalqeId);
            if (halqaId == Guid.Empty)
                return GeneralResponse.NotFound("لا توجد حلقة مرتبطة.");

            var date = request.Date.Date;

            var result = new DailyEntryResultDto
            {
                StudentId = student.Id,
                Date = date,
                ExtraPoints = request.ExtraPoints
            };

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (request.AttendanceStatus.HasValue)
                {
                    var session = await EnsureHalqaSessionAsync(halqaId, date);
                    var lesson = await EnsureSaturdayLessonAsync(student.SaturdayHalqeId);

                    var attendance = await _context.Attendances.FirstOrDefaultAsync(a =>
                        a.StudentId == student.Id && a.HalqeSessionId == session.Id);

                    var note = MergeNotes(request.AttendanceNote, request.TeacherNotes);

                    if (attendance == null)
                    {
                        attendance = new Attendance
                        {
                            Id = Guid.NewGuid(),
                            StudentId = student.Id,
                            TeacherId = teacherId.Value,
                            HalqeSessionId = session.Id,
                            SaturdayLessonId = lesson.Id,
                            Status = request.AttendanceStatus.Value,
                            Note = note
                        };

                        await _context.Attendances.AddAsync(attendance);
                    }
                    else
                    {
                        attendance.Status = request.AttendanceStatus.Value;
                        attendance.Note = note;
                    }

                    result.AttendanceId = attendance.Id;
                }

                if (request.Memorization != null)
                {
                    var entry = MapProgressEntry(request.Memorization, date, teacherId.Value, halqaId, student.Id, isReview: false, planned: false);
                    await _context.ProgressEntries.AddAsync(entry);
                    result.MemorizationEntryId = entry.Id;
                }

                if (request.Review != null)
                {
                    var entry = MapProgressEntry(request.Review, date, teacherId.Value, halqaId, student.Id, isReview: true, planned: false);
                    await _context.ProgressEntries.AddAsync(entry);
                    result.ReviewEntryId = entry.Id;
                }

                if (request.ReviewRequiredForTomorrow != null)
                {
                    var entry = MapRequiredReview(request.ReviewRequiredForTomorrow, date.AddDays(1), teacherId.Value, halqaId, student.Id);
                    await _context.ProgressEntries.AddAsync(entry);
                    result.TomorrowReviewEntryId = entry.Id;
                }

                if (request.Exam != null)
                {
                    var exam = new Exam
                    {
                        Id = Guid.NewGuid(),
                        StudentId = student.Id,
                        TeacherId = teacherId.Value,
                        juz_form = request.Exam.JuzFrom,
                        juz_to = request.Exam.JuzTo,
                        score = request.Exam.Score,
                        mark = request.Exam.Mark,
                        notes = request.Exam.Notes ?? string.Empty,
                        TeacherExamId = request.Exam.TeacherExamId ?? Guid.Empty,
                        date = date
                    };

                    await _context.Exams.AddAsync(exam);
                    result.ExamId = exam.Id;
                }

                if (request.ExtraPoints > 0)
                    student.score += request.ExtraPoints;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return GeneralResponse.Ok("تم حفظ بيانات اليوم.", result);
            }
            catch
            {
                await transaction.RollbackAsync();
                return GeneralResponse.InternalError("حدث خطأ أثناء حفظ بيانات اليوم.");
            }
        }

        public async Task<GeneralResponse> UpdateProgressRecordAsync(UpdateProgressRecordRequest request)
        {
            if (request == null || request.RecordId == Guid.Empty)
                return GeneralResponse.BadRequest("معرف السجل مطلوب.");

            switch (request.RecordType)
            {
                case ProgressRecordType.Exam:
                    {
                        var exam = await _context.Exams.FindAsync(request.RecordId);
                        if (exam == null)
                            return GeneralResponse.NotFound("سجل الاختبار غير موجود.");

                        exam.juz_form = request.JuzFrom ?? exam.juz_form;
                        exam.juz_to = request.JuzTo ?? exam.juz_to;
                        exam.score = request.Score ?? exam.score;
                        exam.mark = request.Mark ?? exam.mark;
                        exam.notes = request.Notes ?? exam.notes;
                        exam.date = request.Date?.Date ?? exam.date;
                        exam.TeacherExamId = request.TeacherExamId ?? exam.TeacherExamId;

                        await _context.SaveChangesAsync();
                        return GeneralResponse.Ok("تم تحديث سجل الاختبار.");
                    }

                case ProgressRecordType.Memorization:
                case ProgressRecordType.Review:
                    {
                        var entry = await _context.ProgressEntries.FindAsync(request.RecordId);
                        if (entry == null)
                            return GeneralResponse.NotFound("سجل التقدّم غير موجود.");

                        entry.JuzNumber = request.JuzNumber ?? entry.JuzNumber;
                        entry.PageNumber = request.FromPage ?? entry.PageNumber;
                        entry.MemorizedUntil = request.ToPage ?? entry.MemorizedUntil;

                        if (request.Grade.HasValue)
                            entry.LevelScore = MapGradeToPoints(request.Grade.Value);

                        entry.NextTarget = request.RecordType == ProgressRecordType.Review ? 0 : Math.Max(entry.MemorizedUntil + 1, 0);
                        entry.Date = request.Date?.Date ?? entry.Date;

                        await _context.SaveChangesAsync();
                        return GeneralResponse.Ok("تم تحديث السجل.");
                    }

                default:
                    return GeneralResponse.BadRequest("نوع السجل غير صالح.");
            }
        }

        public async Task<GeneralResponse> GetStudentProgressSummaryAsync(GetStudentProgressSummaryRequest request)
        {
            if (request == null || request.StudentId == Guid.Empty)
                return GeneralResponse.BadRequest("معرف الطالب مطلوب.");

            var fromDate = (request.FromDate ?? DateTime.UtcNow.AddDays(-30)).Date;
            var toDate = (request.ToDate ?? DateTime.UtcNow).Date;

            var entries = await _context.ProgressEntries
                .AsNoTracking()
                .Where(p => p.StudentId == request.StudentId && p.Date >= fromDate && p.Date <= toDate && p.LevelScore > 0)
                .ToListAsync();

            var exams = await _context.Exams
                .AsNoTracking()
                .Where(e => e.StudentId == request.StudentId && e.date >= fromDate && e.date <= toDate)
                .ToListAsync();

            var summary = new StudentProgressSummaryDto
            {
                StudentId = request.StudentId,
                FromDate = fromDate,
                ToDate = toDate,
                Points = entries.Sum(e => e.LevelScore) + exams.Sum(e => e.score),
                ExamsCount = exams.Count,
                ReviewCount = entries.Count(e => e.NextTarget == 0),
                MemorizationCount = entries.Count(e => e.NextTarget > 0),
                SessionsCount = entries.Count
            };

            return GeneralResponse.Ok("تم جلب ملخص التقدم.", summary);
        }

        public async Task<GeneralResponse> GetStudentProgressHistoryAsync(GetStudentProgressHistoryRequest request)
        {
            if (request == null || request.StudentId == Guid.Empty)
                return GeneralResponse.BadRequest("معرف الطالب مطلوب.");

            var fromDate = (request.FromDate ?? DateTime.UtcNow.AddDays(-30)).Date;
            var toDate = (request.ToDate ?? DateTime.UtcNow).Date;

            var entries = await _context.ProgressEntries
                .AsNoTracking()
                .Where(p => p.StudentId == request.StudentId && p.Date >= fromDate && p.Date <= toDate && p.LevelScore > 0)
                .ToListAsync();

            var exams = await _context.Exams
                .AsNoTracking()
                .Where(e => e.StudentId == request.StudentId && e.date >= fromDate && e.date <= toDate)
                .ToListAsync();

            var records = new List<StudentProgressRecordDto>();
            records.AddRange(entries.Select(MapProgressEntry));
            records.AddRange(exams.Select(MapExam));

            if (request.RecordType.HasValue)
                records = records.Where(r => r.RecordType == request.RecordType.Value).ToList();

            records = records.OrderByDescending(r => r.Date).ToList();

            var pageNumber = request.PageNumber > 0 ? request.PageNumber : 1;
            var pageSize = request.PageSize > 0 ? request.PageSize : 20;

            var totalCount = records.Count;
            var paged = records.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            return GeneralResponse.Ok("تم جلب السجل التفصيلي.", paged, pageNumber, pageSize, totalCount);
        }

        public async Task<GeneralResponse> GetCirclePerformanceOverviewAsync(GetCirclePerformanceOverviewRequest request)
        {
            var teacherId = _currentUserService.CurrentUserId;
            if (!teacherId.HasValue)
                return GeneralResponse.Unauthorized("غير مصرح.");

            var fromDate = (request?.FromDate ?? DateTime.UtcNow.AddDays(-30)).Date;
            var toDate = (request?.ToDate ?? DateTime.UtcNow).Date;

            var progressQuery = _context.ProgressEntries.AsNoTracking()
                .Where(p => p.Date >= fromDate && p.Date <= toDate && p.LevelScore > 0);

            var examQuery = _context.Exams.AsNoTracking()
                .Where(e => e.date >= fromDate && e.date <= toDate);

            var attendanceQuery = _context.Attendances.AsNoTracking()
                .Include(a => a.HalqeSession)
                .Where(a => a.HalqeSession.date >= fromDate && a.HalqeSession.date <= toDate);

            if (request?.CircleId.HasValue == true && request.CircleId != Guid.Empty)
            {
                var circleId = request.CircleId.Value;
                progressQuery = progressQuery.Where(p => p.HalqaId == circleId);
                attendanceQuery = attendanceQuery.Where(a => a.HalqeSession.HalqaId == circleId);

                var studentIds = await _context.Students
                    .Where(s => s.SaturdayHalqeId == circleId)
                    .Select(s => s.Id)
                    .ToListAsync();

                examQuery = examQuery.Where(e => studentIds.Contains(e.StudentId));
            }
            else
            {
                progressQuery = progressQuery.Where(p => p.TeacherId == teacherId.Value);
                attendanceQuery = attendanceQuery.Where(a => a.TeacherId == teacherId.Value);
                examQuery = examQuery.Where(e => e.TeacherId == teacherId.Value);
            }

            var progressEntries = await progressQuery.ToListAsync();
            var exams = await examQuery.ToListAsync();
            var attendances = await attendanceQuery.ToListAsync();

            var chart = BuildChart(progressEntries, exams, attendances);

            var pointsByStudent = new Dictionary<Guid, int>();
            foreach (var entry in progressEntries)
                pointsByStudent[entry.StudentId] = pointsByStudent.GetValueOrDefault(entry.StudentId) + entry.LevelScore;

            foreach (var exam in exams)
                pointsByStudent[exam.StudentId] = pointsByStudent.GetValueOrDefault(exam.StudentId) + exam.score;

            var studentIdsList = pointsByStudent.Keys.ToList();
            var studentNames = await _context.Students
                .Where(s => studentIdsList.Contains(s.Id))
                .Select(s => new { s.Id, s.name })
                .ToListAsync();

            var nameLookup = studentNames.ToDictionary(s => s.Id, s => s.name);

            var ranked = pointsByStudent
                .Select(kvp => new StudentProgressRankDto
                {
                    StudentId = kvp.Key,
                    StudentName = nameLookup.GetValueOrDefault(kvp.Key, "غير معروف"),
                    Points = kvp.Value
                })
                .OrderByDescending(r => r.Points)
                .ToList();

            var overview = new CirclePerformanceOverviewDto
            {
                CircleId = request?.CircleId,
                FromDate = fromDate,
                ToDate = toDate,
                Chart = chart,
                TopStudents = ranked.Take(5).ToList(),
                LaggingStudents = ranked.OrderBy(r => r.Points).Take(5).ToList()
            };

            return GeneralResponse.Ok("تم جلب لوحة الأداء.", overview);
        }

        private async Task<Guid> ResolveHalqaIdAsync(Guid teacherId, Guid? fallbackHalqaId)
        {
            var halqaId = await _context.Halqas
                .Where(h => h.TeacherId == teacherId)
                .Select(h => h.Id)
                .FirstOrDefaultAsync();

            return halqaId != Guid.Empty ? halqaId : (fallbackHalqaId ?? Guid.Empty);
        }

        private async Task<HalqaSession> EnsureHalqaSessionAsync(Guid halqaId, DateTime date)
        {
            var session = await _context.HalqaSessions
                .FirstOrDefaultAsync(s => s.HalqaId == halqaId && s.date == date);

            if (session != null)
                return session;

            session = new HalqaSession
            {
                Id = Guid.NewGuid(),
                HalqaId = halqaId,
                date = date,
                start_time = TimeSpan.Zero,
                end_time = TimeSpan.Zero
            };

            await _context.HalqaSessions.AddAsync(session);
            return session;
        }

        private async Task<SaturdayLesson> EnsureSaturdayLessonAsync(Guid saturdayHalqaId)
        {
            var lesson = await _context.Set<SaturdayLesson>()
                .FirstOrDefaultAsync(l => l.SaturdayHalqeId == saturdayHalqaId && l.lesson_number == 0);

            if (lesson != null)
                return lesson;

            lesson = new SaturdayLesson
            {
                Id = Guid.NewGuid(),
                SaturdayHalqeId = saturdayHalqaId,
                lesson_number = 0,
                start_time = TimeSpan.Zero,
                end_time = TimeSpan.Zero
            };

            await _context.Set<SaturdayLesson>().AddAsync(lesson);
            return lesson;
        }

        private static ProgressEntry MapProgressEntry(MemorizationEntryDto dto, DateTime date, Guid teacherId, Guid halqaId, Guid studentId, bool isReview, bool planned)
        {
            return new ProgressEntry
            {
                Id = Guid.NewGuid(),
                StudentId = studentId,
                TeacherId = teacherId,
                HalqaId = halqaId,
                Date = date,
                JuzNumber = dto.JuzNumber,
                PageNumber = dto.FromPage,
                MemorizedUntil = dto.ToPage,
                NextTarget = isReview ? 0 : Math.Max(dto.ToPage + 1, 0),
                LevelScore = planned ? 0 : MapGradeToPoints(dto.Grade)
            };
        }

        private static ProgressEntry MapProgressEntry(ReviewEntryDto dto, DateTime date, Guid teacherId, Guid halqaId, Guid studentId, bool isReview, bool planned)
            => MapProgressEntry(new MemorizationEntryDto
            {
                JuzNumber = dto.JuzNumber,
                FromPage = dto.FromPage,
                ToPage = dto.ToPage,
                Grade = dto.Grade
            }, date, teacherId, halqaId, studentId, isReview, planned);

        private static ProgressEntry MapRequiredReview(RequiredReviewEntryDto dto, DateTime date, Guid teacherId, Guid halqaId, Guid studentId)
        {
            return new ProgressEntry
            {
                Id = Guid.NewGuid(),
                StudentId = studentId,
                TeacherId = teacherId,
                HalqaId = halqaId,
                Date = date,
                JuzNumber = dto.JuzNumber,
                PageNumber = dto.FromPage,
                MemorizedUntil = dto.ToPage,
                NextTarget = 0,
                LevelScore = 0
            };
        }

        private static StudentProgressRecordDto MapProgressEntry(ProgressEntry entry)
        {
            var isReview = entry.NextTarget == 0;
            var type = isReview ? ProgressRecordType.Review : ProgressRecordType.Memorization;

            return new StudentProgressRecordDto
            {
                RecordId = entry.Id,
                RecordType = type,
                Date = entry.Date,
                Title = isReview ? "مراجعة يومية" : "تسميع جديد",
                JuzNumber = entry.JuzNumber,
                FromPage = entry.PageNumber,
                ToPage = entry.MemorizedUntil,
                Points = entry.LevelScore,
                GradeLabel = ResolveGradeLabel(entry.LevelScore)
            };
        }

        private static StudentProgressRecordDto MapExam(Exam exam)
        {
            return new StudentProgressRecordDto
            {
                RecordId = exam.Id,
                RecordType = ProgressRecordType.Exam,
                Date = exam.date,
                Title = "اختبار مرحلي",
                JuzFrom = exam.juz_form,
                JuzTo = exam.juz_to,
                Score = exam.score,
                Points = exam.score,
                GradeLabel = ResolveExamGradeLabel(exam.score),
                Notes = exam.notes
            };
        }

        private static List<CirclePerformancePointDto> BuildChart(
            List<ProgressEntry> progressEntries,
            List<Exam> exams,
            List<Attendance> attendances)
        {
            var dates = progressEntries.Select(e => e.Date.Date)
                .Concat(exams.Select(e => e.date.Date))
                .Concat(attendances.Select(a => a.HalqeSession.date.Date))
                .Distinct()
                .OrderBy(d => d)
                .ToList();

            return dates.Select(date => new CirclePerformancePointDto
            {
                Date = date,
                MemorizationCount = progressEntries.Count(e => e.Date.Date == date && e.NextTarget > 0),
                ExamsCount = exams.Count(e => e.date.Date == date),
                AttendanceCount = attendances.Count(a => a.HalqeSession.date.Date == date)
            }).ToList();
        }

        private static int MapGradeToPoints(Grade grade) =>
            grade switch
            {
                Grade.Excellent => 5,
                Grade.VeryGood => 4,
                Grade.Good => 3,
                Grade.Acceptable => 2,
                _ => 1
            };

        private static string ResolveGradeLabel(int score) =>
            score switch
            {
                5 => "ممتاز",
                4 => "جيد جدًا",
                3 => "جيد",
                2 => "مقبول",
                _ => "ضعيف"
            };

        private static string ResolveExamGradeLabel(int score) =>
            score switch
            {
                >= 90 => "ممتاز",
                >= 80 => "جيد جدًا",
                >= 70 => "جيد",
                >= 60 => "مقبول",
                _ => "ضعيف"
            };

        private static string? MergeNotes(string? note, string? teacherNotes)
        {
            if (string.IsNullOrWhiteSpace(note))
                return string.IsNullOrWhiteSpace(teacherNotes) ? null : teacherNotes;

            if (string.IsNullOrWhiteSpace(teacherNotes))
                return note;

            return $"{note} | {teacherNotes}";
        }
    }
}