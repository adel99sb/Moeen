using Microsoft.EntityFrameworkCore;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Core.Entities;
using Moeen.Api.infrastructure.Data;
using Moeen.Shared.Constants;
using Moeen.Shared.Responses;
using Moeen.Shared.Responses.Mobile;

namespace Moeen.Api.Application.Services
{
    public class StudentMobileDashboardService : IStudentMobileDashboardService
    {
        private readonly AppDbContext _context;

        public StudentMobileDashboardService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<GeneralResponse> GetDashboardAsync(Guid studentId)
        {
            if (studentId == Guid.Empty)
                return GeneralResponse.BadRequest("معرف الطالب مطلوب.");

            var student = await _context.Students
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == studentId);

            if (student == null)
                return GeneralResponse.NotFound("الطالب غير موجود.");

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

            var response = new StudentDashboardResponse
            {
                StudentId = student.Id,
                StudentName = student.name ?? string.Empty,
                Stats = new StudentDashboardStatsDto
                {
                    TotalPoints = student.score,
                    TotalMemorizations = totalMemorizations,
                    CompletedReviewJuzCount = completedReviewJuzCount,
                    CompletedExamsCount = completedExamsCount
                },
                DailyAssignments = todayAssignments.Select(MapAssignment).ToList(),
                RecentActivities = BuildRecentActivities(recentProgress, recentExams),
                Alerts = BuildAlerts(todayAssignments, recentProgress, absentDaysThisWeek)
            };

            return GeneralResponse.Ok("تم جلب بيانات الصفحة الرئيسية للطالب.", response);
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
                    ProgressRecordType.Review => entry.JuzNumber > 0 ? $"مراجعة الجزء {entry.JuzNumber}" : "مراجعة يومية",
                    _ => "استماع الدرس اليومي"
                },
                Subtitle = isCompleted
                    ? $"تم التقييم بنجاح - {entry.LevelScore} نقاط"
                    : "بانتظار التسليم أو التقييم",
                StatusText = isCompleted ? "مكتمل" : "غير مكتمل",
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
                        ProgressRecordType.Memorization => $"إتمام تسميع صفحة {entry.PageNumber}",
                        ProgressRecordType.Review => entry.JuzNumber > 0 ? $"إتمام مراجعة الجزء {entry.JuzNumber}" : "إتمام مراجعة يومية",
                        _ => "إتمام نشاط استماع"
                    },
                    Subtitle = $"تم التقييم بنجاح - {entry.LevelScore} نقاط | {FormatRelativeDate(entry.Date)}",
                    StatusText = "مكتمل",
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
                StatusText = "مكتمل",
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
                    Subtitle = $"{absentDaysThisWeek} أيام غياب خلال آخر 7 أيام",
                    StatusText = "تنبيه",
                    IsCompleted = false,
                    StatusKind = "danger",
                    IconCssClass = "bi bi-person-x"
                });
            }

            if (recentProgress.Count == 0)
            {
                alerts.Add(new StudentDashboardItemDto
                {
                    Title = "لا يوجد نشاط حديث",
                    Subtitle = "لم يتم تسجيل تسميع أو مراجعة خلال آخر 30 يوم",
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
                    Title = "الالتزام بالجدول",
                    Subtitle = "تم استكمال جميع مهام اليوم",
                    StatusText = "مكتمل",
                    IsCompleted = true,
                    StatusKind = "success",
                    IconCssClass = "bi bi-check2-circle"
                });
            }

            if (alerts.Count == 0)
            {
                alerts.Add(new StudentDashboardItemDto
                {
                    Title = "لا توجد تنبيهات حالياً",
                    Subtitle = "كل شيء يبدو جيداً في الصفحة الرئيسية",
                    StatusText = "جيد",
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
    }
}
