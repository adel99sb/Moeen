using System.Collections.Generic;

namespace Moeen.Shared.Responses.ExamQuery
{
    public class HalqaComparisonDto
    {
        public List<HalqaComparisonItemDto> Items { get; set; } = new();
    }

    public class HalqaComparisonItemDto
    {
        public string HalqaName { get; set; } = string.Empty;
        public int TotalExams { get; set; }
        public double AverageScore { get; set; }
        public double SuccessRate { get; set; }
        public Guid HalqaId { get; set; }
    }
}