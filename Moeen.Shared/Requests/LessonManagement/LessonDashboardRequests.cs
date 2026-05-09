using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.LessonManagement
{
    public class GetWeeklyLessonDashboardRequest
    {
        public DateTime? Date { get; set; }
    }

    public class GetLessonHistoryRequest
    {
        [Required]
        public Guid CircleId { get; set; }

        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public class GetStudentDailyLessonsRequest
    {
        [Required]
        public Guid StudentId { get; set; }

        [Required]
        public DateTime Date { get; set; }
    }

    public class GetCirclesOverviewRequest
    {
        public Guid? MosqueId { get; set; }
    }
}