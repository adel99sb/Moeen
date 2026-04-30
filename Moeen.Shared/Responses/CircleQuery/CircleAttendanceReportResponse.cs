using System;
using System.Collections.Generic;

namespace Moeen.Shared.Responses.CircleQuery
{
    public class CircleAttendanceReportResponse
    {
        public Guid CircleId { get; set; }
        public string CircleName { get; set; } = string.Empty;

        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public List<CircleAttendanceDailyRecordDto> DailyRecords { get; set; } = new();
    }

    public class CircleAttendanceDailyRecordDto
    {
        public DateTime Date { get; set; }

        public int PresentCount { get; set; }
        public int AbsentCount { get; set; }
        public int LateCount { get; set; }
        public int ExcusedCount { get; set; }
    }
}