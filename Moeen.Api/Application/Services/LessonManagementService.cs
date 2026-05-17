using Microsoft.EntityFrameworkCore;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Core.Contracts.infrastructure.Providers;
using Moeen.Api.Core.Entities;
using Moeen.Api.infrastructure.Data;
using Moeen.Shared.Constants;
using Moeen.Shared.Requests.LessonManagement;
using Moeen.Shared.Responses;
using Moeen.Shared.Responses.LessonManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Moeen.Api.Application.Services
{
    public class LessonManagementService : ILessonManagementService
    {
        private readonly AppDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public LessonManagementService(AppDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<GeneralResponse> CreateLessonAsync(CreateLessonRequest request)
        {
            if (request?.LessonData == null || request.LessonData.HalqaId == null)
                return GeneralResponse.BadRequest("بيانات الدرس غير مكتملة.");

            var order = request.LessonData.Order ?? 1;
            var duration = request.LessonData.Duration ?? TimeSpan.Zero;

            var lesson = new SaturdayLesson
            {
                Id = Guid.NewGuid(),
                SaturdayHalqeId = request.LessonData.HalqaId.Value,
                lesson_number = order,
                start_time = TimeSpan.Zero,
                end_time = duration
            };

            await _context.Set<SaturdayLesson>().AddAsync(lesson);
            await _context.SaveChangesAsync();

            return GeneralResponse.Ok("تم إنشاء الدرس.", MapLesson(lesson));
        }

        public async Task<GeneralResponse> UpdateLessonAsync(UpdateLessonRequest request)
        {
            if (request?.LessonData == null || request.Id == Guid.Empty)
                return GeneralResponse.BadRequest("بيانات الدرس غير مكتملة.");

            var lesson = await _context.Set<SaturdayLesson>().FindAsync(request.Id);
            if (lesson == null)
                return GeneralResponse.NotFound("الدرس غير موجود.");

            if (request.LessonData.Order.HasValue)
                lesson.lesson_number = request.LessonData.Order.Value;

            if (request.LessonData.Duration.HasValue)
                lesson.end_time = request.LessonData.Duration.Value;

            await _context.SaveChangesAsync();

            return GeneralResponse.Ok("تم تحديث الدرس.", MapLesson(lesson));
        }

        public async Task<GeneralResponse> DeleteLessonAsync(DeleteLessonRequest request)
        {
            if (request == null || request.Id == Guid.Empty)
                return GeneralResponse.BadRequest("معرف الدرس مطلوب.");

            var hasAttendance = await _context.Attendances.AnyAsync(a => a.SaturdayLessonId == request.Id);
            if (hasAttendance)
                return GeneralResponse.BadRequest("لا يمكن حذف الدرس لوجود سجل حضور.");

            var lesson = await _context.Set<SaturdayLesson>().FindAsync(request.Id);
            if (lesson == null)
                return GeneralResponse.NotFound("الدرس غير موجود.");

            _context.Set<SaturdayLesson>().Remove(lesson);
            await _context.SaveChangesAsync();

            return GeneralResponse.Ok("تم حذف الدرس.");
        }

        public async Task<GeneralResponse> GetLessonsByCircleAsync(GetLessonsByCircleRequest request)
        {
            if (request == null || request.CircleId == Guid.Empty)
                return GeneralResponse.BadRequest("معرف الحلقة مطلوب.");

            IQueryable<SaturdayLesson> query = _context.Set<SaturdayLesson>()
                .AsNoTracking()
                .Where(l => l.SaturdayHalqeId == request.CircleId);

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                if (int.TryParse(request.Search, out int searchNumber))
                    query = query.Where(l => l.lesson_number == searchNumber);
            }

            query = query.OrderBy(l => l.lesson_number);

            var totalCount = await query.CountAsync();
            var pageNumber = request.PageNumber > 0 ? request.PageNumber : 1;
            var pageSize = request.PageSize > 0 ? request.PageSize : 20;

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return GeneralResponse.Ok("تم جلب الدروس.", items.Select(MapLesson).ToList(), pageNumber, pageSize, totalCount);
        }

        public async Task<GeneralResponse> GetWeeklyLessonDashboardAsync(GetWeeklyLessonDashboardRequest request)
        {
            var teacherId = _currentUserService.CurrentUserId;
            if (!teacherId.HasValue)
                return GeneralResponse.Unauthorized("غير مصرح.");

            var date = (request?.Date ?? DateTime.UtcNow).Date;

            var circles = await _context.Set<SaturdayHalqa>()
                .Include(c => c.Students)
                .Include(c => c.SaturdayLessons)
                .Where(c => c.TeacherId == teacherId.Value)
                .ToListAsync();

            var attendances = await _context.Attendances
                .Include(a => a.HalqeSession)
                .Where(a => a.HalqeSession.date == date)
                .ToListAsync();

            var dashboard = new WeeklyLessonDashboardDto
            {
                Date = date,
                Circles = circles.Select(circle =>
                {
                    var currentLesson = circle.SaturdayLessons
                        .OrderByDescending(l => l.lesson_number)
                        .FirstOrDefault();

                    var students = circle.Students.Select(student =>
                    {
                        var status = attendances.FirstOrDefault(a =>
                            a.StudentId == student.Id && a.SaturdayLessonId == currentLesson?.Id);

                        return new LessonStudentStatusDto
                        {
                            StudentId = student.Id,
                            StudentName = student.name,
                            Status = status?.Status.ToString() ?? "غير مسجل",
                            Points = student.score
                        };
                    }).ToList();

                    return new CircleLessonCardDto
                    {
                        CircleId = circle.Id,
                        CircleName = circle.name,
                        CurrentLessonId = currentLesson?.Id,
                        CurrentLessonTitle = currentLesson == null ? "لا يوجد" : $"الدرس {currentLesson.lesson_number}",
                        Students = students
                    };
                }).ToList()
            };

            return GeneralResponse.Ok("تم جلب لوحة الدروس الأسبوعية.", dashboard);
        }

        public async Task<GeneralResponse> RecordLessonAttendanceAsync(RecordAttendanceRequest request)
        {
            if (request == null || request.LessonId == Guid.Empty || request.Entries.Count == 0)
                return GeneralResponse.BadRequest("بيانات الحضور غير مكتملة.");

            var teacherId = _currentUserService.CurrentUserId;
            if (!teacherId.HasValue)
                return GeneralResponse.Unauthorized("غير مصرح.");

            var lesson = await _context.Set<SaturdayLesson>().FindAsync(request.LessonId);
            if (lesson == null)
                return GeneralResponse.NotFound("الدرس غير موجود.");

            var halqaId = ResolveHalqaId(lesson.SaturdayHalqeId);
            if (halqaId == Guid.Empty)
                return GeneralResponse.BadRequest("لا توجد حلقة مرتبطة بالدرس.");

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var session = await EnsureHalqaSessionAsync(halqaId, request.Date.Date);

                var studentIds = request.Entries.Select(e => e.StudentId).Distinct().ToList();

                var existingAttendances = await _context.Attendances
                    .Where(a => a.SaturdayLessonId == lesson.Id && a.HalqeSessionId == session.Id && studentIds.Contains(a.StudentId))
                    .ToListAsync();

                var attendanceLookup = existingAttendances.ToDictionary(a => a.StudentId);
                var newAttendances = new List<Attendance>();

                foreach (var entry in request.Entries)
                {
                    if (attendanceLookup.TryGetValue(entry.StudentId, out var attendance))
                    {
                        attendance.Status = entry.Status;
                        attendance.Note = entry.Note;
                        continue;
                    }

                    newAttendances.Add(new Attendance
                    {
                        Id = Guid.NewGuid(),
                        StudentId = entry.StudentId,
                        TeacherId = teacherId.Value,
                        HalqeSessionId = session.Id,
                        SaturdayLessonId = lesson.Id,
                        Status = entry.Status,
                        Note = entry.Note
                    });
                }

                if (newAttendances.Count > 0)
                    await _context.Attendances.AddRangeAsync(newAttendances);

                var pointsLookup = request.Entries
                    .Where(e => e.Points.HasValue && e.Points.Value > 0)
                    .GroupBy(e => e.StudentId)
                    .ToDictionary(g => g.Key, g => g.Sum(x => x.Points!.Value));

                if (pointsLookup.Count > 0)
                {
                    var students = await _context.Students
                        .Where(s => pointsLookup.Keys.Contains(s.Id))
                        .ToListAsync();

                    foreach (var student in students)
                        student.score += pointsLookup[student.Id];
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return GeneralResponse.Ok("تم تسجيل الحضور.");
            }
            catch
            {
                await transaction.RollbackAsync();
                return GeneralResponse.InternalError("حدث خطأ أثناء تسجيل الحضور.");
            }
        }

        public async Task<GeneralResponse> GetLessonHistoryAsync(GetLessonHistoryRequest request)
        {
            if (request == null || request.CircleId == Guid.Empty)
                return GeneralResponse.BadRequest("معرف الحلقة مطلوب.");

            var fromDate = (request.FromDate ?? DateTime.UtcNow.AddDays(-30)).Date;
            var toDate = (request.ToDate ?? DateTime.UtcNow).Date;

            var historyQuery =
                from a in _context.Attendances.AsNoTracking()
                join l in _context.Set<SaturdayLesson>().AsNoTracking() on a.SaturdayLessonId equals l.Id
                join s in _context.HalqaSessions.AsNoTracking() on a.HalqeSessionId equals s.Id
                where l.SaturdayHalqeId == request.CircleId && s.date >= fromDate && s.date <= toDate
                group new { a, l, s } by new { a.SaturdayLessonId, s.date, l.lesson_number } into g
                select new LessonHistoryItemDto
                {
                    LessonId = g.Key.SaturdayLessonId,
                    LessonTitle = $"الدرس {g.Key.lesson_number}",
                    Date = g.Key.date,
                    TotalStudents = g.Count(),
                    PresentCount = g.Count(x => x.a.Status == AttendanceStatus.Present),
                    AbsentCount = g.Count(x => x.a.Status == AttendanceStatus.Absent)
                };

            var totalCount = await historyQuery.CountAsync();

            var pageNumber = request.PageNumber > 0 ? request.PageNumber : 1;
            var pageSize = request.PageSize > 0 ? request.PageSize : 20;

            var items = await historyQuery
                .OrderByDescending(x => x.Date)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return GeneralResponse.Ok("تم جلب سجل الدروس.", items, pageNumber, pageSize, totalCount);
        }

        public async Task<GeneralResponse> GetCirclesOverviewAsync(GetCirclesOverviewRequest request)
        {
            var circles = await _context.Set<SaturdayHalqa>()
                .Include(c => c.Teacher)
                .Include(c => c.Students)
                .Include(c => c.SaturdayLessons)
                .Where(c => !request.MosqueId.HasValue || c.MosqueId == request.MosqueId.Value)
                .ToListAsync();

            var result = circles.Select(circle =>
            {
                var lesson = circle.SaturdayLessons.OrderBy(l => l.lesson_number).FirstOrDefault();

                return new CircleOverviewDto
                {
                    CircleId = circle.Id,
                    CircleName = circle.name,
                    AgeRange = $"{circle.age_min}-{circle.age_max}",
                    TeacherId = circle.TeacherId,
                    TeacherName = circle.Teacher?.name ?? "غير محدد",
                    StartTime = lesson?.start_time,
                    EndTime = lesson?.end_time,
                    StudentsCount = circle.Students?.Count ?? 0
                };
            }).ToList();

            return GeneralResponse.Ok("تم جلب الحلقات.", result);
        }

        public async Task<GeneralResponse> GetStudentDailyLessonsAsync(GetStudentDailyLessonsRequest request)
        {
            if (request == null || request.StudentId == Guid.Empty)
                return GeneralResponse.BadRequest("معرف الطالب مطلوب.");

            var student = await _context.Students.FindAsync(request.StudentId);
            if (student == null)
                return GeneralResponse.NotFound("الطالب غير موجود.");

            var circle = await _context.Set<SaturdayHalqa>()
                .Include(c => c.Teacher)
                .FirstOrDefaultAsync(c => c.Id == student.SaturdayHalqeId);

            if (circle == null)
                return GeneralResponse.NotFound("لا توجد حلقة مرتبطة بالطالب.");

            var lessons = await _context.Set<SaturdayLesson>()
                .Where(l => l.SaturdayHalqeId == circle.Id)
                .OrderBy(l => l.lesson_number)
                .ToListAsync();

            var dto = new StudentDailyLessonsDto
            {
                StudentId = student.Id,
                Date = request.Date.Date,
                Lessons = lessons.Select(l => new StudentLessonItemDto
                {
                    LessonId = l.Id,
                    LessonTitle = $"الدرس {l.lesson_number}",
                    CircleName = circle.name,
                    TeacherName = circle.Teacher?.name ?? "غير محدد",
                    StartTime = l.start_time,
                    EndTime = l.end_time
                }).ToList()
            };

            return GeneralResponse.Ok("تم جلب دروس الطالب لليوم.", dto);
        }

        private static Guid ResolveHalqaId(Guid? fallbackHalqaId)
            => fallbackHalqaId ?? Guid.Empty;

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

        private static LessonDto MapLesson(SaturdayLesson lesson)
        {
            return new LessonDto
            {
                Id = lesson.Id,
                Title = $"الدرس {lesson.lesson_number}",
                Description = string.Empty,
                Content = string.Empty,
                Order = lesson.lesson_number,
                HalqaId = lesson.SaturdayHalqeId,
                Duration = lesson.end_time,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }
    }
}