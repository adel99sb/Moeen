using System;

namespace Moeen.Shared.Responses.Analytics
{
    public class AnalyticsReportDto
    {
        public Guid Id { get; set; }
        public string ReportType { get; set; } = string.Empty;
        public string? Title { get; set; }
        public string PayloadJson { get; set; } = string.Empty;

        public Guid? StudentId { get; set; }
        public Guid? TeacherId { get; set; }
        public Guid? CircleId { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}