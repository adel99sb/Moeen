using System;

namespace Moeen.Shared.Responses.Analytics
{
    public class AnalyticsReportSummaryDto
    {
        public Guid Id { get; set; }
        public string ReportType { get; set; } = string.Empty;
        public string? Title { get; set; }

        public Guid? StudentId { get; set; }
        public Guid? TeacherId { get; set; }
        public Guid? CircleId { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}