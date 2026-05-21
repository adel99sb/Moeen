using System;
using System.Collections.Generic;

namespace Moeen.Shared.Responses.Reporting
{
    public class StudentProgressReportDto
    {
        public StudentProgressSummaryDto Summary { get; set; } = new();
        public List<StudentProgressRecordDto> Records { get; set; } = [];
    }

    public class StudentProgressSummaryDto
    {
        public int TotalPoints { get; set; }
        public int ExamsCount { get; set; }
        public int MemorizationCount { get; set; }
        public int ReviewCount { get; set; }
        public int AttendanceCount { get; set; }
    }

    public class StudentProgressRecordDto
    {
        public Guid RecordId { get; set; }
        public string Type { get; set; } = string.Empty; // Exam / Memorization / Review
        public DateTime Date { get; set; }
        public int? JuzFrom { get; set; }
        public int? JuzTo { get; set; }
        public int? PageFrom { get; set; }
        public int? PageTo { get; set; }
        public int? Score { get; set; }
        public int? Points { get; set; }
        public string? Notes { get; set; }
    }
}