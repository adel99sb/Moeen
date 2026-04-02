using System;
using System.Collections.Generic;

namespace Moeen.Api.Shared.Responses.Reporting
{
    public class ComparisonReportDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<PeriodData> Periods { get; set; }
    }

    public class PeriodData
    {
        public string PeriodName { get; set; } // e.g., "Week 1", "Month 1"
        public double AttendanceRate { get; set; }
        public double AveragePerformance { get; set; }
        public int TotalStudents { get; set; }
    }
}