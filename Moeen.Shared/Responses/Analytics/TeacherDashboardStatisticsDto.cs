using System;

namespace Moeen.Shared.Responses.Analytics
{
    public class TeacherDashboardStatisticsDto
    {
        public Guid TeacherId { get; set; }
        public string TeacherName { get; set; } = string.Empty;
        public int StudentsCount { get; set; }
        public int HalaqasCount { get; set; }
        public double MemorizationRate { get; set; }
        public double AttendanceRate { get; set; }
        public int TotalPoints { get; set; }
    }
}