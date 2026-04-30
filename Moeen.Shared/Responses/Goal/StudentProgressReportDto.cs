using System;

namespace Moeen.Shared.Responses.Goal
{
    public class StudentProgressReportDto
    {
        public Guid StudentId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public int TotalGoals { get; set; }
        public int AchievedGoals { get; set; }
        public int UnachievedGoals { get; set; }
        public double AchievementRate { get; set; }
    }
}