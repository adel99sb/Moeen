using System;
using System.Collections.Generic;

namespace Moeen.Shared.Responses.Analytics
{
    public class TeacherAnalyticsDto
    {
        public Guid TeacherId { get; set; }
        public string TeacherName { get; set; } = string.Empty;
        public int AssignedHalaqasCount { get; set; }
        public int TotalStudentsCount { get; set; }
        public double AverageStudentAttendance { get; set; }
        public double AverageStudentProgress { get; set; } // average pages per student
        public double AverageExamScore { get; set; }
        public int TotalPointsEarnedByStudents { get; set; }
        public List<HalaqaPerformance> HalaqasPerformance { get; set; } = new List<HalaqaPerformance>();
    }

    public class HalaqaPerformance
    {
        public Guid HalaqaId { get; set; }
        public string HalaqaName { get; set; } = string.Empty;
        public int StudentsCount { get; set; }
        public double AverageAttendance { get; set; }
        public double AverageProgress { get; set; }
    }
}