using System;
using System.Collections.Generic;

namespace Moeen.Shared.Responses.Analytics
{
    public class StudentAnalyticsDto
    {
        public Guid StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Gender { get; set; } = string.Empty;
        public DateTime EnrollmentDate { get; set; }
        public int TotalSessions { get; set; }
        //public double AttendanceRate { get; set; }
        public int TotalMemorizedPages { get; set; }
        public int LastMemorizedPage { get; set; }

        /// متوسط الدرجات في اختبارات هذا الشهر
        public double AverageExamScore { get; set; }
        public int TotalPoints { get; set; }
        public List<MonthlyProgress> MonthlyProgress { get; set; } = new List<MonthlyProgress>();
        public string PerformanceTrend { get; set; } = string.Empty; // "Improving", "Stable", "Declining"
    }

    public class MonthlyProgress
    {
        public DateTime Month { get; set; }
        public int PagesMemorized { get; set; }
        public double AverageScore { get; set; }
    }
}