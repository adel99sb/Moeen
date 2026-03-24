using System;

namespace Moeen.Api.Shared.Responses.Reporting
{
    public class SchedulePeriodicReportResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public Guid? ScheduleId { get; set; }
    }
}