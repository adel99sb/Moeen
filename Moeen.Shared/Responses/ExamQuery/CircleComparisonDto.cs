using System.Collections.Generic;

namespace Moeen.Shared.Responses.ExamQuery
{
    public class CircleComparisonDto
    {
        public List<CircleComparisonItemDto> Items { get; set; } = new();
    }

    public class CircleComparisonItemDto
    {
        public string CircleName { get; set; } = string.Empty;
        public int TotalExams { get; set; }
        public double AverageScore { get; set; }
        public double SuccessRate { get; set; }
    }
}