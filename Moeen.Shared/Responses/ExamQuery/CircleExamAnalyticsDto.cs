using System;
using System.Collections.Generic;

namespace Moeen.Shared.Responses.ExamQuery
{
    public class CircleExamAnalyticsDto
    {
        public Guid CircleId { get; set; }
        public string CircleName { get; set; } = string.Empty;
        public double AverageScore { get; set; }
        public double SuccessRate { get; set; }
        public List<StudentExamPerformanceDto> TopStudents { get; set; } = new();
        public List<StudentExamPerformanceDto> LowStudents { get; set; } = new();
    }

    public class StudentExamPerformanceDto
    {
        public Guid StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public double AverageScore { get; set; }
    }
}