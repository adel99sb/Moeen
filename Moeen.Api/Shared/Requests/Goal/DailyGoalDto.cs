using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.Goal
{
    public class DailyGoalDto
    {
        [Range(0, 10, ErrorMessage = "New pages must be between 0 and 10")]
        public int NewPages { get; set; } = 1; // عدد صفحات الحفظ الجديدة

        [Range(0, 10, ErrorMessage = "Review juz must be between 0 and 10")]
        public int ReviewJuz { get; set; } = 2; // عدد أجزاء المراجعة

        [Range(0, 10, ErrorMessage = "Review pages must be between 0 and 20")]
        public int ReviewPages { get; set; } // عدد صفحات المراجعة (اختياري)

        [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
        public string Notes { get; set; }
    }
}