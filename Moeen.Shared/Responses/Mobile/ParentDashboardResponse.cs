using System;
using System.Collections.Generic;

namespace Moeen.Shared.Responses.Mobile
{
    public class ParentDashboardResponse
    {
        public Guid ParentId { get; set; }
        public string ParentName { get; set; } = string.Empty;
        public Guid? SelectedStudentId { get; set; }
        public string SelectedStudentName { get; set; } = string.Empty;
        public List<ParentChildDto> Children { get; set; } = new();
        public StudentDashboardStatsDto Stats { get; set; } = new();
        public List<StudentDashboardItemDto> DailyAssignments { get; set; } = new();
        public List<StudentDashboardItemDto> RecentActivities { get; set; } = new();
        public List<StudentDashboardItemDto> Alerts { get; set; } = new();
    }

    public class ParentChildDto
    {
        public Guid StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string StudentInitials { get; set; } = string.Empty;
        public string? MosqueName { get; set; }
        public string? HalqaName { get; set; }
        public int TotalPoints { get; set; }
        public int Status { get; set; }
    }
}
