using System;

namespace Moeen.Shared.Responses.Memorization
{
    public class StudentPerformanceDto
    {
        public Guid StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public int TotalPagesMemorized { get; set; }
        public double AverageScore { get; set; }
    }
}