using System;

namespace Moeen.Shared.Responses.HalqaQuery
{
    public class HalqaStatisticsDto
    {
        public Guid HalqaId { get; set; }
        public string HalqaName { get; set; } = string.Empty;

        public int StudentsCount { get; set; }
        public int ActiveStudentsCount { get; set; }

        public double AverageMemorizationProgress { get; set; }
        public double AttendanceRate { get; set; }
        public double AverageEvaluationScore { get; set; }
    }
}