using System;

namespace Moeen.Shared.Responses.CircleQuery
{
    public class CircleStatisticsDto
    {
        public Guid CircleId { get; set; }
        public string CircleName { get; set; } = string.Empty;

        public int StudentsCount { get; set; }
        public int ActiveStudentsCount { get; set; }

        public double AverageMemorizationProgress { get; set; }
        public double AttendanceRate { get; set; }
        public double AverageEvaluationScore { get; set; }
    }
}