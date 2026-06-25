using Microsoft.EntityFrameworkCore;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Core.Entities;
using Moeen.Api.infrastructure.Data;
using Moeen.Shared.Constants;
using Moeen.Shared.Responses;
using Moeen.Shared.Responses.Mobile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Moeen.Api.Application.Services
{
    public class StudentMobileProfileService : IStudentMobileProfileService
    {
        private readonly AppDbContext _context;

        public StudentMobileProfileService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<GeneralResponse> GetProfileAsync(Guid studentId)
        {
            if (studentId == Guid.Empty)
                return GeneralResponse.BadRequest("معرف الطالب مطلوب.");

            var student = await _context.Students
                .AsNoTracking()
                .Include(s => s.Mosque)
                .Include(s => s.Parent)
                .Include(s => s.Halqa)
                    .ThenInclude(h => h.Fouj)
                .Include(s => s.Halqa)
                    .ThenInclude(h => h.Teacher)
                .FirstOrDefaultAsync(s => s.Id == studentId && s.status == 0);

            if (student == null)
                return GeneralResponse.NotFound("الطالب غير موجود.");

            var memorizationCount = await _context.ProgressEntries
                .AsNoTracking()
                .CountAsync(p => p.StudentId == studentId && !p.IsDeleted && p.NextTarget > 0 && p.LevelScore > 0);

            var reviewCount = await _context.ProgressEntries
                .AsNoTracking()
                .CountAsync(p => p.StudentId == studentId && !p.IsDeleted && p.NextTarget == 0 && p.LevelScore > 0);

            var examCount = await _context.Exams
                .AsNoTracking()
                .CountAsync(e => e.StudentId == studentId);

            var response = new StudentProfileResponse
            {
                StudentId = student.Id,
                StudentName = student.name ?? string.Empty,
                StudentInitials = BuildInitials(student.name),
                Age = student.age,
                Gender = NormalizeGender(student.gender),
                Email = student.Email ?? string.Empty,
                PhoneNumber = student.PhoneNumber ?? string.Empty,
                ParentName = student.Parent?.name ?? string.Empty,
                ParentPhoneNumber = student.Parent?.PhoneNumber ?? string.Empty,
                MosqueName = student.Mosque?.name ?? string.Empty,
                FoujName = student.Halqa?.Fouj?.name ?? string.Empty,
                HalqaName = student.Halqa?.Name ?? string.Empty,
                TeacherName = student.Halqa?.Teacher?.name ?? string.Empty,
                EnrollmentDate = student.EnrollmentDate == default ? null : student.EnrollmentDate,
                TotalPoints = student.score,
                Achievements = BuildAchievements(memorizationCount, reviewCount, examCount, student.score),
                WeeklySchedule = BuildWeeklySchedule(student.Halqa)
            };

            return GeneralResponse.Ok("تم جلب الملف الشخصي للطالب.", response);
        }

        public async Task<GeneralResponse> SubmitNoteAsync(Guid studentId, SubmitStudentProfileNoteRequest request)
        {
            if (studentId == Guid.Empty)
                return GeneralResponse.BadRequest("معرف الطالب مطلوب.");

            var content = request?.Content?.Trim() ?? string.Empty;
            if (content.Length < 5)
                return GeneralResponse.BadRequest("الملاحظة يجب أن تكون 5 أحرف على الأقل.");

            if (content.Length > 1000)
                return GeneralResponse.BadRequest("الملاحظة يجب ألا تتجاوز 1000 حرف.");

            var exists = await _context.Students.AsNoTracking().AnyAsync(s => s.Id == studentId && s.status == 0);
            if (!exists)
                return GeneralResponse.NotFound("الطالب غير موجود.");

            var complaint = new Complaint
            {
                Id = Guid.NewGuid(),
                UserId = studentId,
                Title = "ملاحظة من تطبيق الطالب",
                content = content,
                created_at = DateTime.UtcNow,
                Type = FeedbackType.Suggestion,
                SuggestionStatus = SuggestionStatus.InProgress,
                Status = null,
                Response = null,
                UpdatedAt = null
            };

            await _context.Complaints.AddAsync(complaint);
            await _context.SaveChangesAsync();

            return GeneralResponse.Ok("تم إرسال الملاحظة بنجاح.", new { complaint.Id });
        }

        private static List<string> BuildAchievements(int memorizationCount, int reviewCount, int examCount, int totalPoints)
        {
            var achievements = new List<string>();

            if (memorizationCount > 0)
                achievements.Add($"إتمام {memorizationCount} جلسات تسميع مسجلة");

            if (reviewCount > 0)
                achievements.Add($"إتمام {reviewCount} مراجعات ناجحة");

            if (examCount > 0)
                achievements.Add($"إنجاز {examCount} اختبارات");

            if (totalPoints > 0)
                achievements.Add($"جمع {totalPoints} نقطة حتى الآن");

            if (achievements.Count == 0)
                achievements.Add("لا توجد إنجازات مسجلة بعد");

            return achievements;
        }

        private static List<StudentProfileScheduleItemDto> BuildWeeklySchedule(Halqa? halqa)
        {
            var timeRange = BuildTimeRange(halqa?.Fouj);
            var lessonBase = string.IsNullOrWhiteSpace(halqa?.Name) ? "درس الحلقة" : halqa.Name;

            return new List<StudentProfileScheduleItemDto>
            {
                new() { DayName = "الأحد", LessonTitle = $"تسميع - {lessonBase}", TimeRange = timeRange, StatusText = "قادم", StatusKind = "upcoming" },
                new() { DayName = "الثلاثاء", LessonTitle = $"مراجعة - {lessonBase}", TimeRange = timeRange, StatusText = "قادم", StatusKind = "upcoming" },
                new() { DayName = "الخميس", LessonTitle = $"متابعة الحفظ - {lessonBase}", TimeRange = timeRange, StatusText = "قادم", StatusKind = "upcoming" }
            };
        }

        private static string BuildTimeRange(Fouj? fouj)
        {
            if (fouj == null)
                return "غير محدد";

            var start = fouj.start_time == default ? "غير محدد" : fouj.start_time.ToString("HH:mm");
            var end = fouj.End_time == default ? "غير محدد" : fouj.End_time.ToString(@"hh\:mm");
            return $"{start} - {end}";
        }

        private static string BuildInitials(string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return "ط";

            var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 1)
                return parts[0].Substring(0, 1);

            return string.Concat(parts.Take(2).Select(p => p.Substring(0, 1)));
        }

        private static string NormalizeGender(string? gender)
            => gender?.ToLowerInvariant() switch
            {
                "male" => "ذكر",
                "female" => "أنثى",
                _ => string.IsNullOrWhiteSpace(gender) ? "غير محدد" : gender
            };
    }
}
