using System;
using System.Collections.Generic;

namespace Moeen.Shared.Responses.Mobile
{
    public class StudentDashboardResponse
    {
        public Guid StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public StudentDashboardStatsDto Stats { get; set; } = new();
        public List<StudentDashboardItemDto> DailyAssignments { get; set; } = new();
        public List<StudentDashboardItemDto> RecentActivities { get; set; } = new();
        public List<StudentDashboardItemDto> Alerts { get; set; } = new();
    }

    public class StudentDashboardStatsDto
    {
        public int TotalPoints { get; set; }
        public int TotalMemorizations { get; set; }
        public int CompletedReviewJuzCount { get; set; }
        public int CompletedExamsCount { get; set; }
    }

    public class StudentDashboardItemDto
    {
        public Guid? Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Subtitle { get; set; } = string.Empty;
        public string StatusText { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
        public string IconCssClass { get; set; } = "bi bi-info-circle";
        public string StatusKind { get; set; } = "info";
        public DateTime? Date { get; set; }
    }
}
