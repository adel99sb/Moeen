using System;
using System.Collections.Generic;

namespace Moeen.Shared.Responses.Analytics
{
    public class CircleAnalyticsDto
    {
        public Guid CircleId { get; set; }
        public string CircleName { get; set; }
        public string CircleType { get; set; }
        public Guid TeacherId { get; set; }
        public string TeacherName { get; set; }
        public int StudentsCount { get; set; }
        public int ActiveStudentsCount { get; set; }
        public double AverageAttendanceRate { get; set; }
        public double AverageMemorizationProgress { get; set; } // average pages per student
        public double AverageExamScore { get; set; }
        public double RetentionRate { get; set; } // percentage of students still enrolled after 3 months
        public List<StudentPerformance> TopPerformers { get; set; }
        public List<StudentPerformance> StrugglingStudents { get; set; }
    }

    public class StudentPerformance
    {
        public Guid StudentId { get; set; }
        public string StudentName { get; set; }
        public double AttendanceRate { get; set; }
        public int PagesMemorized { get; set; }
        public double AverageScore { get; set; }
    }
}