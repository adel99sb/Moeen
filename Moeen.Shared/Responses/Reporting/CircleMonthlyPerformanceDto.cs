using System;

namespace Moeen.Shared.Responses.Reporting
{
    public class CircleMonthlyPerformanceDto
    {
        public Guid CircleId { get; set; }
        public string CircleName { get; set; } = string.Empty;
        public int AttendanceCount { get; set; }
        public int MemorizationCount { get; set; }
        public int ReviewCount { get; set; }
        public int ExamCount { get; set; }
    }
}