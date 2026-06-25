using Microsoft.EntityFrameworkCore;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Core.Entities;
using Moeen.Api.infrastructure.Data;
using Moeen.Shared.Constants;
using Moeen.Shared.Responses;
using Moeen.Shared.Responses.Mobile;

namespace Moeen.Api.Application.Services
{
    public class ParentMobileDashboardService : IParentMobileDashboardService
    {
        private readonly AppDbContext _context;

        public ParentMobileDashboardService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<GeneralResponse> GetDashboardAsync(Guid parentId, Guid? childId)
        {
            if (parentId == Guid.Empty)
                return GeneralResponse.BadRequest("معرف ولي الأمر غير صالح.");

            var parent = await _context.Students
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == parentId);

            if (parent == null)
                return GeneralResponse.NotFound("ولي الأمر غير موجود.");

            var children = await _context.Students
                .AsNoTracking()
                .Include(s => s.Mosque)
                .Include(s => s.Halqa)
                .Where(s => s.ParentId == parentId && s.status == 0)
                .OrderBy(s => s.name)
                .ToListAsync();

            var response = new ParentDashboardResponse
            {
                ParentId = parent.Id,
                ParentName = parent.name ?? string.Empty,
                Children = children.Select(MapChild).ToList()
            };

            if (children.Count == 0)
            {
                response.Alerts.Add(new StudentDashboardItemDto
                {
                    Title = "لا يوجد طلاب مرتبطون بهذا الحساب",
                    Subtitle = "يرجى مراجعة إدارة المسجد لربط حساب ولي الأمر بالطالب.",
                    StatusText = "تنبيه",
                    StatusKind = "warning",
                    IconCssClass = "bi bi-exclamation-triangle"
                });

                return GeneralResponse.Ok("لا يوجد طلاب مرتبطون بحساب ولي الأمر.", response);
            }

            var selectedChild = childId.HasValue
                ? children.FirstOrDefault(s => s.Id == childId.Value)
                : children.First();

            if (selectedChild == null)
                return GeneralResponse.Unauthorized("هذا الطالب غير مرتبط بحساب ولي الأمر الحالي.");

            response.SelectedStudentId = selectedChild.Id;
            response.SelectedStudentName = selectedChild.name ?? string.Empty;

            await FillSelectedChildDashboardAsync(response, selectedChild.Id, selectedChild.score);
            return GeneralResponse.Ok("تم جلب لوحة ولي الأمر.", response);
        }

        private async Task FillSelectedChildDashboardAsync(ParentDashboardResponse response, Guid studentId, int totalPoints)
        {
            var today = DateTime.UtcNow.Date;
            var weekStart = today.AddDays(-6);
            var thirtyDaysAgo = today.AddDays(-30);

            var totalMemorizations = await _context.ProgressEntries
                .AsNoTracking()
                .CountAsync(p => p.StudentId == studentId && !p.IsDeleted && p.NextTarget > 0);

            var completedReviewJuzCount = await _context.ProgressEntries
                .AsNoTracking()
                .Where(p => p.StudentId == studentId && !p.IsDeleted && p.NextTarget == 0 && p.LevelScore > 0)
                .Select(p => p.JuzNumber)
                .Distinct()
                .CountAsync();

            var completedExamsCount = await _context.Exams
                .AsNoTracking()
                .CountAsync(e => e.StudentId == studentId);

            var todayAssignments = await _context.ProgressEntries
                .AsNoTracking()
                .Where(p => p.StudentId == studentId && !p.IsDeleted && p.Date.Date == today)
                .OrderBy(p => p.NextTarget)
                .ThenBy(p => p.PageNumber)
                .ToListAsync();

            var recentProgress = await _context.ProgressEntries
                .AsNoTracking()
                .Where(p => p.StudentId == studentId && !p.IsDeleted && p.LevelScore > 0 && p.Date.Date >= thirtyDaysAgo)
                .OrderByDescending(p => p.Date)
                .Take(5)
                .ToListAsync();

            var recentExams = await _context.Exams
                .AsNoTracking()
                .Where(e => e.StudentId == studentId && e.date.Date >= thirtyDaysAgo)
                .OrderByDescending(e => e.date)
                .Take(5)
                .ToListAsync();

            var absentDaysThisWeek = await _context.Attendances
                .AsNoTracking()
                .Include(a => a.HalqeSession)
                .Where(a =>
                    a.StudentId == studentId &&
                    a.Status == AttendanceStatus.Absent &&
                    a.HalqeSession != null &&
                    a.HalqeSession.date.Date >= weekStart &&
                    a.HalqeSession.date.Date <= today)
                .Select(a => a.HalqeSession.date.Date)
                .Distinct()
                .CountAsync();

            response.Stats = new StudentDashboardStatsDto
            {
                TotalPoints = totalPoints,
                TotalMemorizations = totalMemorizations,
                CompletedReviewJuzCount = completedReviewJuzCount,
                CompletedExamsCount = completedExamsCount
            };

            response.DailyAssignments = todayAssignments.Select(MapAssignment).ToList();
            response.RecentActivities = BuildRecentActivities(recentProgress, recentExams);
            response.Alerts = BuildAlerts(todayAssignments, recentProgress, absentDaysThisWeek);
        }

        private static ParentChildDto MapChild(Student student)
        {
            return new ParentChildDto
            {
                StudentId = student.Id,
                StudentName = student.name ?? string.Empty,
                StudentInitials = BuildInitials(student.name),
                MosqueName = student.Mosque?.name,
                HalqaName = student.Halqa?.Name,
                TotalPoints = student.score,
                Status = student.status
            };
        }

        private static StudentDashboardItemDto MapAssignment(ProgressEntry entry)
        {
            var isCompleted = entry.LevelScore > 0;
            var type = ResolveAssignmentType(entry);

            return new StudentDashboardItemDto
            {
                Id = entry.Id,
                Date = entry.Date,
                Title = type switch
                {
                    ProgressRecordType.Memorization => $"حفظ من صفحة {entry.PageNumber} إلى صفحة {entry.MemorizedUntil}",
                    ProgressRecordType.Review => entry.JuzNumber > 0 ? $"مراجعة الجزء {entry.JuzNumber}" : "مراجعة عامة",
                    ProgressRecordType.Listening => "استماع ومتابعة",
                    _ => "واجب يومي"
                },
                Subtitle = isCompleted
                    ? $"تم التقييم - {entry.LevelScore} نقاط"
                    : "بانتظار المتابعة من المعلم",
                StatusText = isCompleted ? "منجز" : "قيد المتابعة",
                IsCompleted = isCompleted,
                StatusKind = isCompleted ? "success" : "warning",
                IconCssClass = isCompleted ? "bi bi-check-lg" : "bi bi-exclamation-circle"
            };
        }

        private static List<StudentDashboardItemDto> BuildRecentActivities(
            IReadOnlyList<ProgressEntry> progressEntries,
            IReadOnlyList<Exam> exams)
        {
            var activities = new List<StudentDashboardItemDto>();

            activities.AddRange(progressEntries.Select(entry =>
            {
                var type = ResolveAssignmentType(entry);
                return new StudentDashboardItemDto
                {
                    Id = entry.Id,
                    Date = entry.Date,
                    Title = type switch
                    {
                        ProgressRecordType.Memorization => $"تسميع صفحة {entry.PageNumber}",
                        ProgressRecordType.Review => entry.JuzNumber > 0 ? $"مراجعة الجزء {entry.JuzNumber}" : "مراجعة عامة",
                        ProgressRecordType.Listening => "جلسة استماع",
                        _ => "نشاط جديد"
                    },
                    Subtitle = $"النتيجة: {entry.LevelScore} نقاط | {FormatRelativeDate(entry.Date)}",
                    StatusText = "منجز",
                    IsCompleted = true,
                    StatusKind = "success",
                    IconCssClass = type == ProgressRecordType.Review ? "bi bi-arrow-repeat" : "bi bi-bookmark-check"
                };
            }));

            activities.AddRange(exams.Select(exam => new StudentDashboardItemDto
            {
                Id = exam.Id,
                Date = exam.date,
                Title = exam.juz_form == exam.juz_to
                    ? $"اختبار الجزء {exam.juz_form}"
                    : $"اختبار من الجزء {exam.juz_form} إلى {exam.juz_to}",
                Subtitle = $"العلامة: {exam.mark} - النقاط: {exam.score} | {FormatRelativeDate(exam.date)}",
                StatusText = "منجز",
                IsCompleted = true,
                StatusKind = "success",
                IconCssClass = "bi bi-trophy"
            }));

            return activities
                .OrderByDescending(a => a.Date ?? DateTime.MinValue)
                .Take(5)
                .ToList();
        }

        private static List<StudentDashboardItemDto> BuildAlerts(
            IReadOnlyList<ProgressEntry> todayAssignments,
            IReadOnlyList<ProgressEntry> recentProgress,
            int absentDaysThisWeek)
        {
            var alerts = new List<StudentDashboardItemDto>();

            if (absentDaysThisWeek >= 3)
            {
                alerts.Add(new StudentDashboardItemDto
                {
                    Title = "غياب متكرر",
                    Subtitle = $"تم تسجيل {absentDaysThisWeek} أيام غياب خلال آخر أسبوع.",
                    StatusText = "مهم",
                    IsCompleted = false,
                    StatusKind = "danger",
                    IconCssClass = "bi bi-person-x"
                });
            }

            if (recentProgress.Count == 0)
            {
                alerts.Add(new StudentDashboardItemDto
                {
                    Title = "لا يوجد تقدم حديث",
                    Subtitle = "لم يتم تسجيل تقدم جديد خلال آخر 30 يوم.",
                    StatusText = "تنبيه",
                    IsCompleted = false,
                    StatusKind = "warning",
                    IconCssClass = "bi bi-graph-down-arrow"
                });
            }

            if (todayAssignments.Count > 0 && todayAssignments.All(a => a.LevelScore > 0))
            {
                alerts.Add(new StudentDashboardItemDto
                {
                    Title = "واجبات اليوم مكتملة",
                    Subtitle = "تم إنجاز كل واجبات اليوم بنجاح.",
                    StatusText = "جيد",
                    IsCompleted = true,
                    StatusKind = "success",
                    IconCssClass = "bi bi-check2-circle"
                });
            }

            if (alerts.Count == 0)
            {
                alerts.Add(new StudentDashboardItemDto
                {
                    Title = "لا توجد تنبيهات مهمة",
                    Subtitle = "وضع الطالب مستقر ولا يوجد ما يحتاج متابعة عاجلة.",
                    StatusText = "مستقر",
                    IsCompleted = true,
                    StatusKind = "success",
                    IconCssClass = "bi bi-check2-circle"
                });
            }

            return alerts;
        }

        private static ProgressRecordType ResolveAssignmentType(ProgressEntry entry)
            => entry.NextTarget switch
            {
                0 => ProgressRecordType.Review,
                < 0 => ProgressRecordType.Listening,
                _ => ProgressRecordType.Memorization
            };

        private static string FormatRelativeDate(DateTime date)
        {
            var days = (DateTime.UtcNow.Date - date.Date).Days;

            return days switch
            {
                <= 0 => "اليوم",
                1 => "منذ يوم",
                2 => "منذ يومين",
                <= 10 => $"منذ {days} أيام",
                _ => date.ToString("yyyy/MM/dd")
            };
        }

        private static string BuildInitials(string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return "ط";

            var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (parts.Length == 0)
                return "ط";

            return string.Concat(parts.Take(2).Select(part => part[0])).ToUpperInvariant();
        }
    }
}
