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

            var lesson = await _context.Set<SaturdayLesson>()
                .Include(l => l.StudentLinks)
                .Include(l => l.PdfFiles)
                .FirstOrDefaultAsync(l => l.Id == request.Id);
            if (lesson == null)
                return GeneralResponse.NotFound("الدرس غير موجود.");

            var blockers = new List<string>();

            var attendanceCount = await _context.Attendances.CountAsync(a => a.SaturdayLessonId == request.Id);
            if (attendanceCount > 0)
                blockers.Add($"{attendanceCount} سجل حضور");

            if (lesson.StudentLinks.Count > 0)
                blockers.Add($"{lesson.StudentLinks.Count} طالب مرتبط");

            if (lesson.PdfFiles.Count > 0)
                blockers.Add($"{lesson.PdfFiles.Count} ملف مرتبط");

            if (blockers.Count > 0)
            {
                var blockerText = string.Join("، ", blockers);
                return GeneralResponse.BadRequest($"لا يمكن حذف الدرس لأنه مرتبط ببيانات أخرى: {blockerText}. يرجى إزالة البيانات المرتبطة أولاً.");
            }

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
                return GeneralResponse.Unauthorized("غير مخول.");

            var date = (request?.Date ?? DateTime.UtcNow).Date;

            var lessons = await _context.Set<SaturdayLesson>().AsNoTracking()
                .Include(l => l.WeeklyLesson)
                .Include(l => l.StudentLinks)
                    .ThenInclude(link => link.Student)
                .Where(l => l.WeeklyLessonId != null && l.TeacherId == teacherId.Value)
                .OrderBy(l => l.start_time)
                .ThenBy(l => l.WeeklyLesson != null ? l.WeeklyLesson.Title : string.Empty)
                .ToListAsync();

            var lessonIds = lessons.Select(l => l.Id).ToList();
            var attendances = lessonIds.Count == 0
                ? new List<Attendance>()
                : await _context.Attendances.AsNoTracking()
                    .Include(a => a.HalqeSession)
                    .Where(a => lessonIds.Contains(a.SaturdayLessonId) && a.HalqeSession.date == date)
                    .ToListAsync();

            string ResolveLessonTitle(SaturdayLesson lesson)
                => !string.IsNullOrWhiteSpace(lesson.WeeklyLesson?.Title)
                    ? lesson.WeeklyLesson.Title
                    : $"درس {lesson.lesson_number}";

            var dashboard = new WeeklyLessonDashboardDto
            {
                Date = date,
                Circles = lessons.Select(lesson =>
                {
                    var title = ResolveLessonTitle(lesson);
                    var students = lesson.StudentLinks
                        .OrderBy(link => link.Student != null ? link.Student.name : string.Empty)
                        .Select(link =>
                        {
                            var status = attendances.FirstOrDefault(a => a.StudentId == link.StudentId && a.SaturdayLessonId == lesson.Id);
                            return new LessonStudentStatusDto
                            {
                                StudentId = link.StudentId,
                                StudentName = link.Student?.name ?? string.Empty,
                                Status = status?.Status.ToString() ?? "غير محدد",
                                Points = 0
                            };
                        })
                        .ToList();

                    return new CircleLessonCardDto
                    {
                        CircleId = lesson.Id,
                        CircleName = $"{title} ({lesson.start_time.ToString(@"hh\:mm")} - {lesson.end_time.ToString(@"hh\:mm")})",
                        CurrentLessonId = lesson.Id,
                        CurrentLessonTitle = title,
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
                return GeneralResponse.Unauthorized("غير مخول.");

            var lesson = await _context.Set<SaturdayLesson>()
                .Include(l => l.StudentLinks)
                .FirstOrDefaultAsync(l => l.Id == request.LessonId);

            if (lesson == null)
                return GeneralResponse.NotFound("الدرس غير موجود.");

            var requestedStudentIds = request.Entries.Select(e => e.StudentId).Distinct().ToList();
            Guid sessionHalqaId;
            bool ownsLesson;
            List<Guid> validStudentIds;

            if (lesson.WeeklyLessonId.HasValue)
            {
                ownsLesson = lesson.TeacherId == teacherId.Value;
                if (!ownsLesson)
                    return GeneralResponse.BadRequest("هذا الدرس غير تابع للمعلم الحالي.");

                validStudentIds = lesson.StudentLinks
                    .Where(link => requestedStudentIds.Contains(link.StudentId))
                    .Select(link => link.StudentId)
                    .ToList();

                if (validStudentIds.Count != requestedStudentIds.Count)
                    return GeneralResponse.BadRequest("يوجد طالب غير محدد ضمن هذا الدرس الأسبوعي.");

                sessionHalqaId = lesson.HalqaId ?? await _context.Set<SaturdayLessonStudent>()
                    .AsNoTracking()
                    .Where(link => link.SaturdayLessonId == lesson.Id)
                    .Select(link => link.Student.HalqaId ?? Guid.Empty)
                    .FirstOrDefaultAsync();

                if (sessionHalqaId == Guid.Empty)
                    return GeneralResponse.BadRequest("لا يمكن تسجيل الحضور لأن الطلاب المحددين غير مرتبطين بحلقة مرجعية.");
            }
            else
            {
                sessionHalqaId = await ResolveCanonicalHalqaIdAsync(lesson);
                if (sessionHalqaId == Guid.Empty)
                    return GeneralResponse.BadRequest("لا يمكن تحديد الحلقة التابعة للدرس.");

                ownsLesson = lesson.TeacherId == teacherId.Value
                    || await _context.Halqas.AnyAsync(h => h.Id == sessionHalqaId && h.TeacherId == teacherId.Value)
                    || await _context.Set<SaturdayHalqa>().AnyAsync(h => h.Id == sessionHalqaId && h.TeacherId == teacherId.Value);

                if (!ownsLesson)
                    return GeneralResponse.BadRequest("هذا الدرس غير تابع للمعلم الحالي.");

                validStudentIds = await _context.Students.AsNoTracking()
                    .Where(s => requestedStudentIds.Contains(s.Id) && s.HalqaId == sessionHalqaId)
                    .Select(s => s.Id)
                    .ToListAsync();

                if (validStudentIds.Count != requestedStudentIds.Count)
                    return GeneralResponse.BadRequest("يوجد طالب غير تابع لهذه الحلقة.");
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var session = await EnsureHalqaSessionAsync(sessionHalqaId, request.Date.Date);
                var existingAttendances = await _context.Attendances
                    .Where(a => a.SaturdayLessonId == lesson.Id && a.HalqeSessionId == session.Id && requestedStudentIds.Contains(a.StudentId))
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

                return GeneralResponse.Ok("تم تسجيل الحضور بنجاح.");
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
                return GeneralResponse.BadRequest("معرّف الدرس أو الحلقة غير صالح.");

            var teacherId = _currentUserService.CurrentUserId;
            if (!teacherId.HasValue || !IsCurrentUserTeacher())
                return GeneralResponse.Unauthorized("غير مخول.");

            var requestedLesson = await _context.Set<SaturdayLesson>()
                .AsNoTracking()
                .FirstOrDefaultAsync(l => l.Id == request.CircleId);

            bool filterByLesson = requestedLesson != null;
            if (filterByLesson && requestedLesson!.TeacherId != teacherId.Value)
                return GeneralResponse.BadRequest("هذا الدرس غير تابع للمعلم الحالي.");

            if (!filterByLesson)
            {
                var teacherCircleIds = await GetTeacherCircleIdsAsync(teacherId.Value);
                if (!teacherCircleIds.Contains(request.CircleId))
                    return GeneralResponse.BadRequest("لا يمكنك عرض سجل حلقة غير تابعة لك.");
            }

            var fromDate = (request.FromDate ?? DateTime.UtcNow.AddDays(-30)).Date;
            var toDate = (request.ToDate ?? DateTime.UtcNow).Date;

            var historyQuery =
                from a in _context.Attendances.AsNoTracking()
                join l in _context.Set<SaturdayLesson>().AsNoTracking() on a.SaturdayLessonId equals l.Id
                join s in _context.HalqaSessions.AsNoTracking() on a.HalqeSessionId equals s.Id
                where (filterByLesson
                        ? a.SaturdayLessonId == request.CircleId
                        : ((l.HalqaId.HasValue && l.HalqaId.Value == request.CircleId) || (!l.HalqaId.HasValue && l.SaturdayHalqeId == request.CircleId)))
                    && s.date >= fromDate
                    && s.date <= toDate
                group new { a, l, s } by new { a.SaturdayLessonId, s.date, l.lesson_number, WeeklyTitle = l.WeeklyLesson != null ? l.WeeklyLesson.Title : string.Empty } into g
                select new LessonHistoryItemDto
                {
                    LessonId = g.Key.SaturdayLessonId,
                    LessonTitle = string.IsNullOrWhiteSpace(g.Key.WeeklyTitle) ? $"درس {g.Key.lesson_number}" : g.Key.WeeklyTitle,
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

            var managedMosqueId = await ResolveManagedMosqueIdAsync();
            var effectiveMosqueId = managedMosqueId ?? request.MosqueId;

            var circles = await _context.Set<SaturdayHalqa>()
                .Include(c => c.Teacher)
                .Include(c => c.Students)
                .Include(c => c.SaturdayLessons)
                .Where(c => !effectiveMosqueId.HasValue || c.MosqueId == effectiveMosqueId.Value)
                .ToListAsync();

            var result = circles.Select(circle =>
            {
                var lesson = circle.SaturdayLessons.OrderBy(l => l.lesson_number).FirstOrDefault();

                return new CircleOverviewDto
                {
                    CircleId = circle.Id,
                    CircleName = circle.name,
                    AgeRange = $"{circle.age_min}-{circle.age_max}",
                    TeacherId = circle.TeacherId ?? Guid.Empty,
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

            var managedMosqueId = await ResolveManagedMosqueIdAsync();
            var query = _context.WeeklyLessons
                .AsNoTracking()
                .Include(l => l.Schedules)
                    .ThenInclude(s => s.Teacher)
                .Include(l => l.Schedules)
                    .ThenInclude(s => s.Halqa)
                        .ThenInclude(h => h.Fouj)
                .Include(l => l.Schedules)
                    .ThenInclude(s => s.Halqa)
                        .ThenInclude(h => h.Students)
                .AsQueryable();

            if (managedMosqueId.HasValue)
            {
                query = query.Where(l => l.Schedules.Any(s =>
                    (s.TeacherId.HasValue && s.Teacher.MosqueId == managedMosqueId.Value) ||
                    (s.HalqaId.HasValue && s.Halqa.Fouj.MosqueId == managedMosqueId.Value) ||
                    s.SaturdayHalqe.MosqueId == managedMosqueId.Value));
            }

            var lessons = await query
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
                return GeneralResponse.BadRequest($"لا يمكن حذف الدرس الأسبوعي لأنه مرتبط بـ {lesson.Schedules.Count} موعد. يرجى حذف المواعيد المرتبطة أولاً.");

            _context.WeeklyLessons.Remove(lesson);
            await _context.SaveChangesAsync();

            return GeneralResponse.Ok("تم حذف الدرس الأسبوعي.");
        }

        public async Task<GeneralResponse> CreateWeeklyLessonAssignmentAsync(CreateWeeklyLessonAssignmentRequest request)
        {
            if (!CanManageWeeklyLessons())
                return GeneralResponse.Unauthorized("غير مخول.");

            if (request == null)
                return GeneralResponse.BadRequest("طلب غير صالح.");

            var validation = await ValidateWeeklyLessonAssignmentAsync(
                request.WeeklyLessonId,
                request.TeacherId,
                request.StudentIds,
                request.StartTime,
                request.EndTime);

            if (validation.Error != null)
                return validation.Error;

            try
            {
                var weeklyLesson = validation.WeeklyLesson!;
                var teacher = validation.Teacher!;
                var selectedStudents = validation.Students;
                var legacyContainer = await EnsureWeeklyLessonContainerAsync(weeklyLesson, teacher);
                var assignmentId = Guid.NewGuid();

                var assignment = new SaturdayLesson
                {
                    Id = assignmentId,
                    WeeklyLessonId = weeklyLesson.Id,
                    SaturdayHalqeId = legacyContainer.Id,
                    HalqaId = request.HalqaId,
                    TeacherId = teacher.Id,
                    lesson_number = 1,
                    start_time = request.StartTime,
                    end_time = request.EndTime,
                    StudentLinks = selectedStudents.Select(student => new SaturdayLessonStudent
                    {
                        SaturdayLessonId = assignmentId,
                        StudentId = student.Id,
                        CreatedAt = DateTime.UtcNow
                    }).ToList()
                };

                await _context.Set<SaturdayLesson>().AddAsync(assignment);
                await _context.SaveChangesAsync();

                return GeneralResponse.Ok("تم إنشاء موعد الدرس الأسبوعي بنجاح.");
            }
            catch (DbUpdateException ex) when (TryMapWeeklyLessonAssignmentDbError(ex) is GeneralResponse error)
            {
                return error;
            }
        }

        public async Task<GeneralResponse> UpdateWeeklyLessonAssignmentAsync(UpdateWeeklyLessonAssignmentRequest request)
        {
            if (!CanManageWeeklyLessons())
                return GeneralResponse.Unauthorized("غير مخول.");

            if (request == null || request.Id == Guid.Empty)
                return GeneralResponse.BadRequest("معرّف الموعد غير صالح.");

            var assignment = await _context.Set<SaturdayLesson>()
                .Include(l => l.StudentLinks)
                .FirstOrDefaultAsync(l => l.Id == request.Id && l.WeeklyLessonId != null);

            if (assignment == null)
                return GeneralResponse.NotFound("موعد الدرس الأسبوعي غير موجود.");

            var validation = await ValidateWeeklyLessonAssignmentAsync(
                request.WeeklyLessonId,
                request.TeacherId,
                request.StudentIds,
                request.StartTime,
                request.EndTime,
                request.Id);

            if (validation.Error != null)
                return validation.Error;

            try
            {
                var weeklyLesson = validation.WeeklyLesson!;
                var teacher = validation.Teacher!;
                var legacyContainer = await EnsureWeeklyLessonContainerAsync(weeklyLesson, teacher, assignment.SaturdayHalqeId);

                assignment.WeeklyLessonId = weeklyLesson.Id;
                assignment.SaturdayHalqeId = legacyContainer.Id;
                assignment.HalqaId = request.HalqaId;
                assignment.TeacherId = teacher.Id;
                assignment.start_time = request.StartTime;
                assignment.end_time = request.EndTime;
                SyncWeeklyLessonStudents(assignment, validation.Students.Select(student => student.Id).ToList());

                await _context.SaveChangesAsync();
                return GeneralResponse.Ok("تم تعديل موعد الدرس الأسبوعي بنجاح.");
            }
            catch (DbUpdateException ex) when (TryMapWeeklyLessonAssignmentDbError(ex) is GeneralResponse error)
            {
                return error;
            }
        }

        public async Task<GeneralResponse> DeleteWeeklyLessonAssignmentAsync(Guid assignmentId)
        {
            if (!CanManageWeeklyLessons())
                return GeneralResponse.Unauthorized("غير مخول.");

            if (assignmentId == Guid.Empty)
                return GeneralResponse.BadRequest("معرّف الموعد غير صالح.");

            var assignment = await _context.Set<SaturdayLesson>()
                .Include(l => l.StudentLinks)
                .Include(l => l.PdfFiles)
                .FirstOrDefaultAsync(l => l.Id == assignmentId && l.WeeklyLessonId != null);

            if (assignment == null)
                return GeneralResponse.NotFound("موعد الدرس الأسبوعي غير موجود.");

            var blockers = new List<string>();

            var attendanceCount = await _context.Attendances.CountAsync(a => a.SaturdayLessonId == assignmentId);
            if (attendanceCount > 0)
                blockers.Add($"{attendanceCount} سجل حضور");

            if (assignment.StudentLinks.Count > 0)
                blockers.Add($"{assignment.StudentLinks.Count} طالب مرتبط");

            if (assignment.PdfFiles.Count > 0)
                blockers.Add($"{assignment.PdfFiles.Count} ملف مرتبط");

            if (blockers.Count > 0)
            {
                var blockerText = string.Join("، ", blockers);
                return GeneralResponse.BadRequest($"لا يمكن حذف موعد الدرس الأسبوعي لأنه مرتبط ببيانات أخرى: {blockerText}. يرجى إزالة البيانات المرتبطة أولاً.");
            }

            _context.Set<SaturdayLesson>().Remove(assignment);
            await _context.SaveChangesAsync();

            return GeneralResponse.Ok("تم حذف موعد الدرس الأسبوعي بنجاح.");
        }

        public async Task<GeneralResponse> GetStudentDailyLessonsAsync(GetStudentDailyLessonsRequest request)
        {
            if (request == null || request.StudentId == Guid.Empty)
                return GeneralResponse.BadRequest("معرّف الطالب غير صالح.");

            var student = await _context.Students.FindAsync(request.StudentId);
            if (student == null)
                return GeneralResponse.NotFound("الطالب غير موجود.");

            var weeklyLessons = await _context.Set<SaturdayLesson>()
                .AsNoTracking()
                .Include(l => l.WeeklyLesson)
                .Include(l => l.Teacher)
                .Include(l => l.StudentLinks)
                .Where(l => l.WeeklyLessonId != null && l.StudentLinks.Any(link => link.StudentId == student.Id))
                .OrderBy(l => l.start_time)
                .ToListAsync();

            var dto = new StudentDailyLessonsDto
            {
                StudentId = student.Id,
                Date = request.Date.Date,
                Lessons = weeklyLessons.Select(l => new StudentLessonItemDto
                {
                    LessonId = l.Id,
                    LessonTitle = !string.IsNullOrWhiteSpace(l.WeeklyLesson?.Title) ? l.WeeklyLesson.Title : $"درس {l.lesson_number}",
                    CircleName = "درس أسبوعي",
                    TeacherName = l.Teacher?.name ?? "غير محدد",
                    StartTime = l.start_time,
                    EndTime = l.end_time
                }).ToList()
            };

            return GeneralResponse.Ok("تم جلب دروس الطالب اليومية بنجاح.", dto);
        }

        private async Task<(GeneralResponse? Error, WeeklyLesson? WeeklyLesson, Teacher? Teacher, List<Student> Students)> ValidateWeeklyLessonAssignmentAsync(
            Guid weeklyLessonId,
            Guid teacherId,
            IReadOnlyCollection<Guid>? studentIds,
            TimeSpan startTime,
            TimeSpan endTime,
            Guid? assignmentIdToIgnore = null)
        {
            if (weeklyLessonId == Guid.Empty)
                return (GeneralResponse.BadRequest("يرجى اختيار الدرس الأسبوعي."), null, null, new List<Student>());

            if (teacherId == Guid.Empty)
                return (GeneralResponse.BadRequest("يرجى اختيار المعلم."), null, null, new List<Student>());

            var selectedStudentIds = studentIds?
                .Where(id => id != Guid.Empty)
                .Distinct()
                .ToList() ?? new List<Guid>();

            if (selectedStudentIds.Count == 0)
                return (GeneralResponse.BadRequest("يرجى اختيار طالب واحد على الأقل لهذا الدرس."), null, null, new List<Student>());

            if (endTime <= startTime)
                return (GeneralResponse.BadRequest("وقت النهاية يجب أن يكون بعد وقت البداية."), null, null, new List<Student>());

            var weeklyLesson = await _context.WeeklyLessons.FirstOrDefaultAsync(l => l.Id == weeklyLessonId);
            if (weeklyLesson == null)
                return (GeneralResponse.NotFound("الدرس الأسبوعي غير موجود."), null, null, new List<Student>());

            var teacher = await _context.Teachers.AsNoTracking().FirstOrDefaultAsync(t => t.Id == teacherId);
            if (teacher == null)
                return (GeneralResponse.NotFound("المعلم غير موجود."), null, null, new List<Student>());

            if (teacher.status != 0)
                return (GeneralResponse.BadRequest("لا يمكن تعيين معلم غير نشط على درس أسبوعي."), null, null, new List<Student>());

            var students = await _context.Students
                .AsNoTracking()
                .Where(s => selectedStudentIds.Contains(s.Id))
                .ToListAsync();

            if (students.Count != selectedStudentIds.Count)
                return (GeneralResponse.BadRequest("يوجد طالب محدد غير موجود."), null, null, new List<Student>());

            var managedMosqueId = await ResolveManagedMosqueIdAsync();
            if (managedMosqueId.HasValue)
            {
                if (teacher.MosqueId != managedMosqueId.Value)
                    return (GeneralResponse.BadRequest("لا يمكنك إنشاء درس لمعلم خارج المسجد الذي تديره."), null, null, new List<Student>());

                if (students.Any(s => s.MosqueId != managedMosqueId.Value))
                    return (GeneralResponse.BadRequest("لا يمكنك اختيار طلاب خارج المسجد الذي تديره."), null, null, new List<Student>());
            }

            var teacherHasOverlap = await _context.Set<SaturdayLesson>()
                .AsNoTracking()
                .AnyAsync(l =>
                    l.WeeklyLessonId != null &&
                    l.TeacherId == teacherId &&
                    (!assignmentIdToIgnore.HasValue || l.Id != assignmentIdToIgnore.Value) &&
                    (l.start_time < endTime && startTime < l.end_time));

            if (teacherHasOverlap)
                return (GeneralResponse.BadRequest("لا يمكن إنشاء أكثر من درس أسبوعي بنفس الوقت لنفس المعلم."), null, null, new List<Student>());

            var overlappingStudentName = await _context.Set<SaturdayLessonStudent>()
                .AsNoTracking()
                .Include(link => link.Student)
                .Include(link => link.SaturdayLesson)
                .Where(link =>
                    selectedStudentIds.Contains(link.StudentId) &&
                    link.SaturdayLesson.WeeklyLessonId != null &&
                    (!assignmentIdToIgnore.HasValue || link.SaturdayLessonId != assignmentIdToIgnore.Value) &&
                    (link.SaturdayLesson.start_time < endTime && startTime < link.SaturdayLesson.end_time))
                .Select(link => link.Student.name)
                .FirstOrDefaultAsync();

            if (!string.IsNullOrWhiteSpace(overlappingStudentName))
                return (GeneralResponse.BadRequest($"الطالب {overlappingStudentName} لديه درس أسبوعي آخر بنفس الوقت."), null, null, new List<Student>());

            return (null, weeklyLesson, teacher, students);
        }

        private static bool TimesOverlap(TimeSpan existingStart, TimeSpan existingEnd, TimeSpan newStart, TimeSpan newEnd)
            => existingStart < newEnd && newStart < existingEnd;

        private async Task<SaturdayHalqa> EnsureWeeklyLessonContainerAsync(WeeklyLesson lesson, Teacher teacher, Guid? existingContainerId = null)
        {
            SaturdayHalqa? container = null;

            if (existingContainerId.HasValue && existingContainerId.Value != Guid.Empty)
                container = await _context.Set<SaturdayHalqa>().FirstOrDefaultAsync(h => h.Id == existingContainerId.Value);

            if (container == null)
            {
                container = new SaturdayHalqa
                {
                    Id = Guid.NewGuid(),
                    TeacherId = teacher.Id,
                    name = lesson.Title,
                    age_min = 0,
                    age_max = 0,
                    MosqueId = teacher.MosqueId,
                    SaturdayLessons = new List<SaturdayLesson>(),
                    Students = new List<Student>()
                };

                await _context.Set<SaturdayHalqa>().AddAsync(container);
                return container;
            }

            container.TeacherId = teacher.Id;
            container.name = lesson.Title;
            container.MosqueId = teacher.MosqueId;
            return container;
        }

        private void SyncWeeklyLessonStudents(SaturdayLesson assignment, IReadOnlyCollection<Guid> selectedStudentIds)
        {
            var selected = selectedStudentIds.ToHashSet();
            var removed = assignment.StudentLinks.Where(link => !selected.Contains(link.StudentId)).ToList();
            if (removed.Count > 0)
                _context.Set<SaturdayLessonStudent>().RemoveRange(removed);

            var existing = assignment.StudentLinks.Select(link => link.StudentId).ToHashSet();
            foreach (var studentId in selected.Where(id => !existing.Contains(id)))
            {
                assignment.StudentLinks.Add(new SaturdayLessonStudent
                {
                    SaturdayLessonId = assignment.Id,
                    StudentId = studentId,
                    CreatedAt = DateTime.UtcNow
                });
            }
        }

        private static WeeklyLessonManagementDto MapWeeklyLesson(WeeklyLesson lesson)
        {
            var rows = lesson.Schedules?
                .OrderBy(row => row.start_time)
                .ThenBy(row => row.Teacher != null ? row.Teacher.name : string.Empty)
                .Select(row =>
                {
                    var studentLinks = row.StudentLinks?
                        .OrderBy(link => link.Student != null ? link.Student.name : string.Empty)
                        .ToList() ?? new List<SaturdayLessonStudent>();

                    return new WeeklyLessonAssignmentDto
                    {
                        Id = row.Id,
                        WeeklyLessonId = row.WeeklyLessonId ?? Guid.Empty,
                        TeacherId = row.TeacherId ?? Guid.Empty,
                        TeacherName = row.Teacher != null ? row.Teacher.name : string.Empty,
                        HalqaId = row.HalqaId ?? Guid.Empty,
                        HalqaName = row.Halqa != null ? row.Halqa.Name : string.Empty,
                        FoujName = row.Halqa != null && row.Halqa.Fouj != null ? row.Halqa.Fouj.name : string.Empty,
                        StudentsCount = studentLinks.Count,
                        StudentIds = studentLinks.Select(link => link.StudentId).ToList(),
                        StudentNames = string.Join("، ", studentLinks.Select(link => link.Student != null ? link.Student.name : string.Empty).Where(name => !string.IsNullOrWhiteSpace(name))),
                        StartTime = row.start_time,
                        EndTime = row.end_time
                    };
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
            var currentUserId = _currentUserService.CurrentUserId;
            if (!currentUserId.HasValue)
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
