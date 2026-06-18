using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
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
            if (!teacherId.HasValue || !IsCurrentUserTeacher())
                return GeneralResponse.Unauthorized("غير مصرح.");

            var date = (request?.Date ?? DateTime.UtcNow).Date;

            var regularCircles = await _context.Halqas
                .AsNoTracking()
                .Where(h => h.TeacherId == teacherId.Value)
                .Select(h => new CircleLookup(h.Id, h.Name ?? string.Empty))
                .ToListAsync();

            var regularCircleIds = regularCircles
                .Select(circle => circle.Id)
                .ToHashSet();

            var fallbackCircles = await _context.Set<SaturdayHalqa>()
                .AsNoTracking()
                .Where(h =>
                    h.TeacherId == teacherId.Value &&
                    !regularCircleIds.Contains(h.Id) &&
                    _context.Set<SaturdayLesson>().Any(l => l.TeacherId == teacherId.Value && l.SaturdayHalqeId == h.Id && !l.HalqaId.HasValue))
                .Select(h => new CircleLookup(h.Id, h.name ?? string.Empty))
                .ToListAsync();

            var circles = regularCircles
                .Concat(fallbackCircles)
                .OrderBy(c => c.Name)
                .ToList();

            var circleIds = circles.Select(c => c.Id).ToList();

            var lessons = await _context.Set<SaturdayLesson>().AsNoTracking()
                .Include(l => l.WeeklyLesson)
                .Where(l =>
                    l.TeacherId == teacherId.Value ||
                    (l.HalqaId.HasValue && circleIds.Contains(l.HalqaId.Value)) ||
                    (l.SaturdayHalqeId != Guid.Empty && circleIds.Contains(l.SaturdayHalqeId)))
                .ToListAsync();

            var lessonIds = lessons.Select(l => l.Id).ToList();
            var attendances = lessonIds.Count == 0
                ? new List<Attendance>()
                : await _context.Attendances.AsNoTracking()
                    .Include(a => a.HalqeSession)
                    .Where(a => lessonIds.Contains(a.SaturdayLessonId) && a.HalqeSession.date == date)
                    .ToListAsync();

            var studentRows = regularCircleIds.Count == 0
                ? new List<LessonStudentLookup>()
                : (await _context.Students.AsNoTracking()
                    .Where(s => s.HalqaId.HasValue && regularCircleIds.Contains(s.HalqaId.Value))
                    .Select(s => new
                    {
                        s.Id,
                        Name = s.name ?? string.Empty,
                        s.HalqaId
                    })
                    .ToListAsync())
                    .Select(s => new LessonStudentLookup(s.Id, s.Name, s.HalqaId!.Value))
                    .ToList();

            string ResolveLessonTitle(SaturdayLesson lesson)
                => !string.IsNullOrWhiteSpace(lesson.WeeklyLesson?.Title)
                    ? lesson.WeeklyLesson.Title
                    : $"الدرس {lesson.lesson_number}";

            var dashboard = new WeeklyLessonDashboardDto
            {
                Date = date,
                Circles = circles.Select(circle =>
                {
                    var currentLesson = lessons
                        .Where(l => ResolveLessonCircleId(l) == circle.Id)
                        .OrderBy(l => l.start_time)
                        .ThenBy(l => l.lesson_number)
                        .FirstOrDefault();

                    var students = studentRows
                        .Where(student => student.HalqaId == circle.Id)
                        .OrderBy(student => student.Name)
                        .Select(student =>
                        {
                            var status = attendances.FirstOrDefault(a =>
                                a.StudentId == student.Id && a.SaturdayLessonId == currentLesson?.Id);

                            return new LessonStudentStatusDto
                            {
                                StudentId = student.Id,
                                StudentName = student.Name,
                                Status = status?.Status.ToString() ?? "غير مسجل",
                                Points = 0
                            };
                        })
                        .ToList();

                    return new CircleLessonCardDto
                    {
                        CircleId = circle.Id,
                        CircleName = circle.Name,
                        CurrentLessonId = currentLesson?.Id,
                        CurrentLessonTitle = currentLesson == null ? "لا يوجد" : ResolveLessonTitle(currentLesson),
                        Students = students
                    };
                }).ToList()
            };

            return GeneralResponse.Ok("تم جلب لوحة الدروس الأسبوعية.", dashboard);
        }

        public async Task<GeneralResponse> RecordLessonAttendanceAsync(RecordAttendanceRequest request)
        {
            if (request == null || request.LessonId == Guid.Empty || request.Entries == null || request.Entries.Count == 0)
                return GeneralResponse.BadRequest("بيانات الحضور غير مكتملة.");

            var teacherId = _currentUserService.CurrentUserId;
            if (!teacherId.HasValue || !IsCurrentUserTeacher())
                return GeneralResponse.Unauthorized("غير مصرح.");

            var lesson = await _context.Set<SaturdayLesson>().FirstOrDefaultAsync(l => l.Id == request.LessonId);
            if (lesson == null)
                return GeneralResponse.NotFound("الدرس غير موجود.");

            var halqaId = await ResolveCanonicalHalqaIdAsync(lesson);

            if (halqaId == Guid.Empty)
                return GeneralResponse.BadRequest("لا توجد حلقة مرتبطة بالدرس.");

            var ownsLesson = lesson.TeacherId == teacherId.Value
                || await _context.Halqas.AnyAsync(h => h.Id == halqaId && h.TeacherId == teacherId.Value)
                || await _context.Set<SaturdayHalqa>().AnyAsync(h => h.Id == halqaId && h.TeacherId == teacherId.Value);

            if (!ownsLesson)
                return GeneralResponse.BadRequest("هذا الدرس غير تابع لحلقات المعلم الحالي.");

            var requestedStudentIds = request.Entries.Select(e => e.StudentId).Distinct().ToList();
            var validStudentIds = await _context.Students.AsNoTracking()
                .Where(s => requestedStudentIds.Contains(s.Id) && s.HalqaId == halqaId)
                .Select(s => s.Id)
                .ToListAsync();

            if (validStudentIds.Count != requestedStudentIds.Count)
                return GeneralResponse.BadRequest("يوجد طلاب غير تابعين لهذه الحلقة.");

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

            var teacherId = _currentUserService.CurrentUserId;
            if (!teacherId.HasValue || !IsCurrentUserTeacher())
                return GeneralResponse.Unauthorized("غير مصرح.");

            var teacherCircleIds = await GetTeacherCircleIdsAsync(teacherId.Value);
            if (!teacherCircleIds.Contains(request.CircleId))
                return GeneralResponse.BadRequest("هذه الحلقة غير تابعة للمعلم الحالي.");

            var fromDate = (request.FromDate ?? DateTime.UtcNow.AddDays(-30)).Date;
            var toDate = (request.ToDate ?? DateTime.UtcNow).Date;

            var historyQuery =
                from a in _context.Attendances.AsNoTracking()
                join l in _context.Set<SaturdayLesson>().AsNoTracking() on a.SaturdayLessonId equals l.Id
                join s in _context.HalqaSessions.AsNoTracking() on a.HalqeSessionId equals s.Id
                where ((l.HalqaId.HasValue && l.HalqaId.Value == request.CircleId) || (!l.HalqaId.HasValue && l.SaturdayHalqeId == request.CircleId))
                    && s.date >= fromDate
                    && s.date <= toDate
                group new { a, l, s } by new { a.SaturdayLessonId, s.date, l.lesson_number, WeeklyTitle = l.WeeklyLesson != null ? l.WeeklyLesson.Title : string.Empty } into g
                select new LessonHistoryItemDto
                {
                    LessonId = g.Key.SaturdayLessonId,
                    LessonTitle = string.IsNullOrWhiteSpace(g.Key.WeeklyTitle) ? $"الدرس {g.Key.lesson_number}" : g.Key.WeeklyTitle,
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
            if (!CanManageWeeklyLessons())
                return GeneralResponse.Unauthorized("غير مصرح.");

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

        public async Task<GeneralResponse> GetManagedWeeklyLessonsAsync()
        {
            if (!CanManageWeeklyLessons())
                return GeneralResponse.Unauthorized("غير مصرح.");

            var lessons = await _context.WeeklyLessons
                .AsNoTracking()
                .Include(l => l.Schedules)
                    .ThenInclude(s => s.Teacher)
                .Include(l => l.Schedules)
                    .ThenInclude(s => s.Halqa)
                        .ThenInclude(h => h.Fouj)
                .Include(l => l.Schedules)
                    .ThenInclude(s => s.Halqa)
                        .ThenInclude(h => h.Students)
                .OrderBy(l => l.Title)
                .ToListAsync();

            return GeneralResponse.Ok("تم جلب الدروس الأسبوعية.", lessons.Select(MapWeeklyLesson).ToList());
        }

        public async Task<GeneralResponse> CreateWeeklyLessonAsync(CreateWeeklyLessonRequest request)
        {
            if (!CanManageWeeklyLessons())
                return GeneralResponse.Unauthorized("غير مصرح.");

            if (request == null || string.IsNullOrWhiteSpace(request.Title))
                return GeneralResponse.BadRequest("عنوان الدرس الأسبوعي مطلوب.");

            var title = request.Title.Trim();
            var exists = await _context.WeeklyLessons.AnyAsync(l => l.Title == title);
            if (exists)
                return GeneralResponse.BadRequest("يوجد درس أسبوعي بنفس الاسم مسبقاً.");

            var lesson = new WeeklyLesson
            {
                Id = Guid.NewGuid(),
                Title = title,
                Description = request.Description?.Trim() ?? string.Empty,
                CreatedAt = DateTime.UtcNow
            };

            await _context.WeeklyLessons.AddAsync(lesson);
            await _context.SaveChangesAsync();

            return GeneralResponse.Ok("تم إنشاء الدرس الأسبوعي.", MapWeeklyLesson(lesson));
        }

        public async Task<GeneralResponse> UpdateWeeklyLessonAsync(UpdateWeeklyLessonRequest request)
        {
            if (!CanManageWeeklyLessons())
                return GeneralResponse.Unauthorized("غير مصرح.");

            if (request == null || request.Id == Guid.Empty || string.IsNullOrWhiteSpace(request.Title))
                return GeneralResponse.BadRequest("بيانات الدرس الأسبوعي غير مكتملة.");

            var lesson = await _context.WeeklyLessons.FindAsync(request.Id);
            if (lesson == null)
                return GeneralResponse.NotFound("الدرس الأسبوعي غير موجود.");

            var title = request.Title.Trim();
            var duplicate = await _context.WeeklyLessons.AnyAsync(l => l.Id != request.Id && l.Title == title);
            if (duplicate)
                return GeneralResponse.BadRequest("يوجد درس أسبوعي بنفس الاسم مسبقاً.");

            lesson.Title = title;
            lesson.Description = request.Description?.Trim() ?? string.Empty;
            lesson.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return GeneralResponse.Ok("تم تعديل الدرس الأسبوعي.", MapWeeklyLesson(lesson));
        }

        public async Task<GeneralResponse> DeleteWeeklyLessonAsync(Guid weeklyLessonId)
        {
            if (!CanManageWeeklyLessons())
                return GeneralResponse.Unauthorized("غير مصرح.");

            if (weeklyLessonId == Guid.Empty)
                return GeneralResponse.BadRequest("معرف الدرس الأسبوعي مطلوب.");

            var lesson = await _context.WeeklyLessons
                .Include(l => l.Schedules)
                .FirstOrDefaultAsync(l => l.Id == weeklyLessonId);

            if (lesson == null)
                return GeneralResponse.NotFound("الدرس الأسبوعي غير موجود.");

            if (lesson.Schedules.Count > 0)
                _context.Set<SaturdayLesson>().RemoveRange(lesson.Schedules);

            _context.WeeklyLessons.Remove(lesson);
            await _context.SaveChangesAsync();

            return GeneralResponse.Ok("تم حذف الدرس الأسبوعي.");
        }

        public async Task<GeneralResponse> CreateWeeklyLessonAssignmentAsync(CreateWeeklyLessonAssignmentRequest request)
        {
            if (!CanManageWeeklyLessons())
                return GeneralResponse.Unauthorized("غير مصرح.");

            var validation = await ValidateWeeklyLessonAssignmentAsync(request.WeeklyLessonId, request.TeacherId, request.HalqaId, request.StartTime, request.EndTime);
            if (!validation.Success)
                return validation;

            try
            {
                var legacySaturdayHalqa = await EnsureLegacySaturdayHalqaAsync(request.HalqaId, request.TeacherId);

                var assignment = new SaturdayLesson
                {
                    Id = Guid.NewGuid(),
                    WeeklyLessonId = request.WeeklyLessonId,
                    SaturdayHalqeId = legacySaturdayHalqa.Id,
                    HalqaId = request.HalqaId,
                    TeacherId = request.TeacherId,
                    lesson_number = 1,
                    start_time = request.StartTime,
                    end_time = request.EndTime
                };

                await _context.Set<SaturdayLesson>().AddAsync(assignment);
                await _context.SaveChangesAsync();

                return GeneralResponse.Ok("تم إضافة موعد الحلقة للدرس الأسبوعي.");
            }
            catch (DbUpdateException ex) when (TryMapWeeklyLessonAssignmentDbError(ex) is GeneralResponse error)
            {
                return error;
            }
        }

        public async Task<GeneralResponse> UpdateWeeklyLessonAssignmentAsync(UpdateWeeklyLessonAssignmentRequest request)
        {
            if (!CanManageWeeklyLessons())
                return GeneralResponse.Unauthorized("غير مصرح.");

            if (request == null || request.Id == Guid.Empty)
                return GeneralResponse.BadRequest("معرف الموعد مطلوب.");

            var assignment = await _context.Set<SaturdayLesson>().FirstOrDefaultAsync(l => l.Id == request.Id && l.WeeklyLessonId != null);
            if (assignment == null)
                return GeneralResponse.NotFound("موعد الحلقة غير موجود.");

            var validation = await ValidateWeeklyLessonAssignmentAsync(request.WeeklyLessonId, request.TeacherId, request.HalqaId, request.StartTime, request.EndTime, request.Id);
            if (!validation.Success)
                return validation;

            try
            {
                var legacySaturdayHalqa = await EnsureLegacySaturdayHalqaAsync(request.HalqaId, request.TeacherId);

                assignment.WeeklyLessonId = request.WeeklyLessonId;
                assignment.SaturdayHalqeId = legacySaturdayHalqa.Id;
                assignment.HalqaId = request.HalqaId;
                assignment.TeacherId = request.TeacherId;
                assignment.start_time = request.StartTime;
                assignment.end_time = request.EndTime;

                await _context.SaveChangesAsync();
                return GeneralResponse.Ok("تم تعديل موعد الحلقة.");
            }
            catch (DbUpdateException ex) when (TryMapWeeklyLessonAssignmentDbError(ex) is GeneralResponse error)
            {
                return error;
            }
        }

        public async Task<GeneralResponse> DeleteWeeklyLessonAssignmentAsync(Guid assignmentId)
        {
            if (!CanManageWeeklyLessons())
                return GeneralResponse.Unauthorized("غير مصرح.");

            if (assignmentId == Guid.Empty)
                return GeneralResponse.BadRequest("معرف الموعد مطلوب.");

            var hasAttendance = await _context.Attendances.AnyAsync(a => a.SaturdayLessonId == assignmentId);
            if (hasAttendance)
                return GeneralResponse.BadRequest("لا يمكن حذف الموعد لوجود سجل حضور مرتبط به.");

            var assignment = await _context.Set<SaturdayLesson>().FirstOrDefaultAsync(l => l.Id == assignmentId && l.WeeklyLessonId != null);
            if (assignment == null)
                return GeneralResponse.NotFound("موعد الحلقة غير موجود.");

            _context.Set<SaturdayLesson>().Remove(assignment);
            await _context.SaveChangesAsync();

            return GeneralResponse.Ok("تم حذف موعد الحلقة.");
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

        private async Task<GeneralResponse> ValidateWeeklyLessonAssignmentAsync(Guid weeklyLessonId, Guid teacherId, Guid halqaId, TimeSpan startTime, TimeSpan endTime, Guid? assignmentIdToIgnore = null)
        {
            if (weeklyLessonId == Guid.Empty)
                return GeneralResponse.BadRequest("يرجى اختيار الدرس الأسبوعي.");

            if (teacherId == Guid.Empty)
                return GeneralResponse.BadRequest("يرجى اختيار المعلم.");

            if (halqaId == Guid.Empty)
                return GeneralResponse.BadRequest("يرجى اختيار الحلقة.");

            if (endTime <= startTime)
                return GeneralResponse.BadRequest("وقت النهاية يجب أن يكون بعد وقت البداية.");

            var lessonExists = await _context.WeeklyLessons.AnyAsync(l => l.Id == weeklyLessonId);
            if (!lessonExists)
                return GeneralResponse.NotFound("الدرس الأسبوعي غير موجود.");

            var teacher = await _context.Teachers.AsNoTracking().FirstOrDefaultAsync(t => t.Id == teacherId);
            if (teacher == null)
                return GeneralResponse.NotFound("المعلم غير موجود.");

            var halqa = await _context.Halqas
                .AsNoTracking()
                .Include(h => h.Fouj)
                .FirstOrDefaultAsync(h => h.Id == halqaId);
            if (halqa == null)
                return GeneralResponse.NotFound("الحلقة غير موجودة.");

            if (halqa.TeacherId != teacherId)
                return GeneralResponse.BadRequest("الحلقة المختارة غير تابعة للمعلم المحدد.");

            if (halqa.Fouj == null)
                return GeneralResponse.BadRequest("الحلقة المختارة غير مرتبطة بفوج صالح.");

            if (teacher.MosqueId != Guid.Empty && teacher.MosqueId != halqa.Fouj.MosqueId)
                return GeneralResponse.BadRequest("المعلم المختار لا يتبع مسجد الحلقة المحددة.");

            var managedMosqueId = await ResolveManagedMosqueIdAsync();
            if (managedMosqueId.HasValue && halqa.Fouj.MosqueId != managedMosqueId.Value)
                return GeneralResponse.BadRequest("الحلقة المختارة خارج نطاق المسجد الذي يديره المشرف الحالي.");

            var duplicateExists = await _context.Set<SaturdayLesson>()
                .AsNoTracking()
                .AnyAsync(l =>
                    l.WeeklyLessonId == weeklyLessonId &&
                    l.HalqaId == halqaId &&
                    l.TeacherId == teacherId &&
                    l.start_time == startTime &&
                    l.end_time == endTime &&
                    (!assignmentIdToIgnore.HasValue || l.Id != assignmentIdToIgnore.Value));

            if (duplicateExists)
                return GeneralResponse.BadRequest("يوجد موعد مطابق لهذه الحلقة ضمن الدرس الأسبوعي.");

            return GeneralResponse.Ok("Valid");
        }

        private static WeeklyLessonManagementDto MapWeeklyLesson(WeeklyLesson lesson)
        {
            var rows = lesson.Schedules?
                .OrderBy(row => row.start_time)
                .ThenBy(row => row.Halqa != null ? row.Halqa.Name : string.Empty)
                .Select(row => new WeeklyLessonAssignmentDto
                {
                    Id = row.Id,
                    WeeklyLessonId = row.WeeklyLessonId ?? Guid.Empty,
                    TeacherId = row.TeacherId ?? Guid.Empty,
                    TeacherName = row.Teacher != null ? row.Teacher.name : string.Empty,
                    HalqaId = row.HalqaId ?? row.SaturdayHalqeId,
                    HalqaName = row.Halqa != null ? row.Halqa.Name : string.Empty,
                    FoujName = row.Halqa != null && row.Halqa.Fouj != null ? row.Halqa.Fouj.name : string.Empty,
                    StudentsCount = 0,
                    StartTime = row.start_time,
                    EndTime = row.end_time
                })
                .ToList() ?? new List<WeeklyLessonAssignmentDto>();

            return new WeeklyLessonManagementDto
            {
                Id = lesson.Id,
                Title = lesson.Title,
                Description = lesson.Description ?? string.Empty,
                CreatedAt = lesson.CreatedAt,
                UpdatedAt = lesson.UpdatedAt,
                AssignmentsCount = rows.Count,
                Assignments = rows
            };
        }

        private sealed record CircleLookup(Guid Id, string Name);
        private sealed record LessonStudentLookup(Guid Id, string Name, Guid HalqaId);

        private bool IsCurrentUserTeacher()
            => _currentUserService.IsInRole(nameof(Roles.Teacher));

        private bool CanManageWeeklyLessons()
            => _currentUserService.IsAdmin == true || _currentUserService.IsInRole("Supervisor");

        private async Task<Guid?> ResolveManagedMosqueIdAsync()
        {
            if (_currentUserService.IsAdmin == true)
                return null;

            var currentUserId = _currentUserService.CurrentUserId;
            if (!currentUserId.HasValue || !_currentUserService.IsInRole("Supervisor"))
                return null;

            return await _context.Supervisors
                .AsNoTracking()
                .Where(s => s.Id == currentUserId.Value)
                .Select(s => (Guid?)s.MosqueId)
                .FirstOrDefaultAsync();
        }

        private async Task<SaturdayHalqa> EnsureLegacySaturdayHalqaAsync(Guid halqaId, Guid teacherId)
        {
            var legacySaturdayHalqa = await _context.Set<SaturdayHalqa>()
                .FirstOrDefaultAsync(h => h.Id == halqaId);

            var halqa = await _context.Halqas
                .Include(h => h.Fouj)
                .FirstOrDefaultAsync(h => h.Id == halqaId);

            if (halqa == null || halqa.Fouj == null)
                throw new InvalidOperationException("Halqa must exist before creating a weekly lesson assignment.");

            if (legacySaturdayHalqa == null)
            {
                legacySaturdayHalqa = new SaturdayHalqa
                {
                    Id = halqa.Id,
                    TeacherId = teacherId,
                    name = halqa.Name ?? string.Empty,
                    age_min = 0,
                    age_max = 0,
                    MosqueId = halqa.Fouj.MosqueId,
                    SaturdayLessons = new List<SaturdayLesson>(),
                    Students = new List<Student>()
                };

                await _context.Set<SaturdayHalqa>().AddAsync(legacySaturdayHalqa);
                return legacySaturdayHalqa;
            }

            legacySaturdayHalqa.TeacherId = teacherId;
            legacySaturdayHalqa.name = string.IsNullOrWhiteSpace(halqa.Name) ? legacySaturdayHalqa.name : halqa.Name;
            legacySaturdayHalqa.MosqueId = halqa.Fouj.MosqueId;
            return legacySaturdayHalqa;
        }

        private static GeneralResponse? TryMapWeeklyLessonAssignmentDbError(DbUpdateException ex)
        {
            if (ex.InnerException is not SqlException sqlException)
                return null;

            return sqlException.Number switch
            {
                547 => GeneralResponse.BadRequest("تعذر حفظ موعد الحلقة لأن بعض البيانات المرتبطة غير صالحة أو غير مكتملة."),
                2601 or 2627 => GeneralResponse.BadRequest("يوجد موعد مطابق مسبقاً لهذه الحلقة."),
                _ => null
            };
        }

        private Guid ResolveLessonCircleId(SaturdayLesson lesson)
        {
            if (lesson.HalqaId.HasValue && lesson.HalqaId.Value != Guid.Empty)
                return lesson.HalqaId.Value;

            return lesson.SaturdayHalqeId;
        }

        private async Task<Guid> ResolveCanonicalHalqaIdAsync(SaturdayLesson lesson)
        {
            if (lesson.HalqaId.HasValue && lesson.HalqaId.Value != Guid.Empty)
                return lesson.HalqaId.Value;

            if (lesson.SaturdayHalqeId == Guid.Empty)
                return Guid.Empty;

            var halqaId = await _context.Halqas
                .AsNoTracking()
                .Where(h => h.Id == lesson.SaturdayHalqeId)
                .Select(h => h.Id)
                .FirstOrDefaultAsync();

            return halqaId;
        }

        private async Task<HashSet<Guid>> GetTeacherCircleIdsAsync(Guid teacherId)
        {
            var halqaIds = await _context.Halqas
                .AsNoTracking()
                .Where(h => h.TeacherId == teacherId)
                .Select(h => h.Id)
                .ToListAsync();

            var fallbackSaturdayIds = await _context.Set<SaturdayHalqa>()
                .AsNoTracking()
                .Where(h =>
                    h.TeacherId == teacherId &&
                    !halqaIds.Contains(h.Id) &&
                    _context.Set<SaturdayLesson>().Any(l => l.TeacherId == teacherId && l.SaturdayHalqeId == h.Id && !l.HalqaId.HasValue))
                .Select(h => h.Id)
                .ToListAsync();

            return halqaIds
                .Concat(fallbackSaturdayIds)
                .ToHashSet();
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
