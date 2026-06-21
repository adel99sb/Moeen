using Microsoft.EntityFrameworkCore;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Core.Entities;
using Moeen.Api.infrastructure.Data;
using Moeen.Shared.Constants;
using Moeen.Shared.Responses;
using Moeen.Shared.Responses.Mobile;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Moeen.Api.Application.Services
{
    public class StudentMobileProgressService : IStudentMobileProgressService
    {
        private readonly AppDbContext _context;

        public StudentMobileProgressService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<GeneralResponse> GetProgressAsync(Guid studentId, DateTime? from, DateTime? to, ProgressRecordType? type)
        {
            if (studentId == Guid.Empty)
                return GeneralResponse.BadRequest("معرف الطالب مطلوب.");

            var student = await _context.Students
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == studentId);

            if (student == null)
                return GeneralResponse.NotFound("الطالب غير موجود.");

            var today = DateTime.UtcNow.Date;
            var fromDate = (from ?? today.AddDays(-30)).Date;
            var toDate = (to ?? today).Date;

            if (fromDate > toDate)
                return GeneralResponse.BadRequest("تاريخ البداية يجب أن يكون قبل تاريخ النهاية.");

            var progressEntries = await _context.ProgressEntries
                .AsNoTracking()
                .Where(p =>
                    p.StudentId == studentId &&
                    !p.IsDeleted &&
                    p.Date.Date >= fromDate &&
                    p.Date.Date <= toDate)
                .OrderByDescending(p => p.Date)
                .ToListAsync();

            var exams = await _context.Exams
                .AsNoTracking()
                .Where(e =>
                    e.StudentId == studentId &&
                    e.date.Date >= fromDate &&
                    e.date.Date <= toDate)
                .OrderByDescending(e => e.date)
                .ToListAsync();

            var progressRecords = progressEntries
                .Select(MapProgressEntry)
                .Where(r => type == null || r.Type == type.Value)
                .ToList();

            var examRecords = exams
                .Select(MapExam)
                .Where(r => type == null || r.Type == type.Value)
                .ToList();

            var records = progressRecords
                .Concat(examRecords)
                .OrderByDescending(r => r.Date)
                .ThenByDescending(r => r.Points)
                .Take(50)
                .ToList();

            var memorizationSessions = progressEntries.Count(p => ResolveProgressType(p) == ProgressRecordType.Memorization);
            var reviewSessions = progressEntries.Count(p => ResolveProgressType(p) == ProgressRecordType.Review);
            var pointsInPeriod = progressEntries.Sum(p => Math.Max(0, p.LevelScore)) + exams.Sum(e => Math.Max(0, e.score));
            var normalizedScores = BuildNormalizedScores(progressEntries, exams);
            var averageScore = normalizedScores.Count == 0 ? 0 : (int)Math.Round(normalizedScores.Average());

            var response = new StudentProgressResponse
            {
                StudentId = student.Id,
                StudentName = student.name ?? string.Empty,
                StudentInitials = BuildInitials(student.name),
                StudentNumber = BuildStudentNumber(student.Id),
                RegisteredSince = student.EnrollmentDate == default ? null : student.EnrollmentDate,
                Stats = new StudentProgressStatsDto
                {
                    MemorizationSessions = memorizationSessions,
                    ReviewSessions = reviewSessions,
                    ExamsCount = exams.Count,
                    TotalPoints = student.score
                },
                Records = records,
                Summary = new StudentProgressSummaryDto
                {
                    PeriodTitle = BuildPeriodTitle(fromDate, toDate),
                    MemorizationCompletionPercentage = CalculatePercentage(memorizationSessions, 24),
                    ReviewCompletionPercentage = CalculatePercentage(reviewSessions, 18),
                    AverageRatingPercentage = averageScore,
                    AverageRatingText = RatingFromNormalizedScore(averageScore),
                    PointsInPeriod = pointsInPeriod,
                    PointsInPeriodPercentage = CalculatePercentage(pointsInPeriod, 100)
                }
            };

            return GeneralResponse.Ok("تم جلب سجل تقدم الطالب.", response);
        }

        private static StudentProgressRecordDto MapProgressEntry(ProgressEntry entry)
        {
            var type = ResolveProgressType(entry);
            var points = Math.Max(0, entry.LevelScore);
            var normalizedScore = Math.Min(100, points * 10);

            return new StudentProgressRecordDto
            {
                Id = entry.Id,
                Type = type,
                TypeText = TypeText(type),
                Date = entry.Date,
                JuzNumber = entry.JuzNumber > 0 ? entry.JuzNumber : null,
                FromPage = entry.PageNumber > 0 ? entry.PageNumber : null,
                ToPage = entry.MemorizedUntil > 0 ? entry.MemorizedUntil : null,
                Points = points,
                RatingText = RatingFromNormalizedScore(normalizedScore),
                StatusKind = StatusKindFromScore(normalizedScore),
                TeacherNote = string.Empty,
                Title = type switch
                {
                    ProgressRecordType.Memorization => "تسميع جديد",
                    ProgressRecordType.Review => "مراجعة يومية",
                    ProgressRecordType.Listening => "استماع الدرس",
                    _ => "سجل تقدم"
                }
            };
        }

        private static StudentProgressRecordDto MapExam(Exam exam)
        {
            var normalizedScore = Math.Clamp(exam.mark, 0, 100);

            return new StudentProgressRecordDto
            {
                Id = exam.Id,
                Type = ProgressRecordType.Exam,
                TypeText = TypeText(ProgressRecordType.Exam),
                Title = "اختبار مرحلي",
                Date = exam.date,
                JuzFrom = exam.juz_form,
                JuzTo = exam.juz_to,
                Mark = exam.mark,
                Points = Math.Max(0, exam.score),
                RatingText = RatingFromNormalizedScore(normalizedScore),
                StatusKind = StatusKindFromScore(normalizedScore),
                TeacherNote = exam.notes ?? string.Empty
            };
        }

        private static ProgressRecordType ResolveProgressType(ProgressEntry entry)
            => entry.NextTarget switch
            {
                0 => ProgressRecordType.Review,
                < 0 => ProgressRecordType.Listening,
                _ => ProgressRecordType.Memorization
            };

        private static string TypeText(ProgressRecordType type)
            => type switch
            {
                ProgressRecordType.Memorization => "تسميع",
                ProgressRecordType.Review => "مراجعة",
                ProgressRecordType.Exam => "اختبار",
                ProgressRecordType.Listening => "استماع",
                _ => "سجل"
            };

        private static List<int> BuildNormalizedScores(IEnumerable<ProgressEntry> progressEntries, IEnumerable<Exam> exams)
        {
            var scores = progressEntries
                .Where(p => p.LevelScore > 0)
                .Select(p => Math.Min(100, p.LevelScore * 10))
                .ToList();

            scores.AddRange(exams.Select(e => Math.Clamp(e.mark, 0, 100)));
            return scores;
        }

        private static string RatingFromNormalizedScore(int score)
            => score switch
            {
                >= 90 => "ممتاز",
                >= 75 => "جيد جداً",
                >= 60 => "جيد",
                > 0 => "يحتاج متابعة",
                _ => "غير مكتمل"
            };

        private static string StatusKindFromScore(int score)
            => score switch
            {
                >= 90 => "excellent",
                >= 75 => "verygood",
                >= 60 => "good",
                > 0 => "warning",
                _ => "repeat"
            };

        private static int CalculatePercentage(int value, int target)
        {
            if (target <= 0)
                return 0;

            return Math.Clamp((int)Math.Round(value * 100.0 / target), 0, 100);
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

        private static string BuildStudentNumber(Guid studentId)
            => studentId.ToString("N")[..8].ToUpperInvariant();

        private static string BuildPeriodTitle(DateTime from, DateTime to)
        {
            var culture = new CultureInfo("ar");
            if (from.Month == to.Month && from.Year == to.Year)
                return to.ToString("MMMM yyyy", culture);

            return $"{from:yyyy/MM/dd} - {to:yyyy/MM/dd}";
        }
    }
}
