using System;
using System.Collections.Generic;

namespace Moeen.Shared.Responses.Analytics
{
    public class HalqaAnalyticsDto
    {
        public Guid HalqaId { get; set; }
        public string HalqaName { get; set; } = string.Empty;
        public string HalqaType { get; set; } = string.Empty;
        public Guid TeacherId { get; set; }
        public string TeacherName { get; set; } = string.Empty;
        public int StudentsCount { get; set; }
        public int ActiveStudentsCount { get; set; }
        public double AverageAttendanceRate { get; set; }
        public double AverageMemorizationProgress { get; set; } // average pages per student
        public double AverageExamScore { get; set; }
        public double RetentionRate { get; set; } // percentage of students still enrolled after 3 months
        public List<StudentPerformance> TopPerformers { get; set; } = new List<StudentPerformance>();
        public List<StudentPerformance> StrugglingStudents { get; set; } = new List<StudentPerformance>();
    }

    public class StudentPerformance
    {
        public Guid StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public double AttendanceRate { get; set; }
        public int PagesMemorized { get; set; }
        public double AverageScore { get; set; }
    }
}