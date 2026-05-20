using System;
using System.Collections.Generic;

namespace Moeen.Shared.Responses.HalqaQuery
{
    public class HalqaAttendanceReportResponse
    {
        public Guid HalqaId { get; set; }
        public string HalqaName { get; set; } = string.Empty;

        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public List<HalqaAttendanceDailyRecordDto> DailyRecords { get; set; } = new();
    }

    public class HalqaAttendanceDailyRecordDto
    {
        public DateTime Date { get; set; }

        public int PresentCount { get; set; }
        public int AbsentCount { get; set; }
        public int LateCount { get; set; }
        public int ExcusedCount { get; set; }
    }
}