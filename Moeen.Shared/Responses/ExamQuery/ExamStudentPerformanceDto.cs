using System;

namespace Moeen.Shared.Responses.ExamQuery
{
    public class ExamStudentPerformanceDto
    {
        public Guid StudentId { get; set; }
        public string? StudentName { get; set; }
        public int ExamsTaken { get; set; }
        public double AverageGrade { get; set; }
        public int TotalPoints { get; set; }
    }
}   