using System;

namespace Moeen.Shared.Requests.Analytics
{
    public class AnalyticsReportFilter
    {
        public string? ReportType { get; set; }
        public Guid? StudentId { get; set; }
        public Guid? TeacherId { get; set; }
        public Guid? CircleId { get; set; }

        public DateTime? From { get; set; }
        public DateTime? To { get; set; }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}