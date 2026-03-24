using System;

namespace Moeen.Api.Shared.Responses.Reporting
{
    public class ReportDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Type { get; set; } // "Attendance" or "Performance"
        public byte[] Content { get; set; } // PDF أو Excel
        public string ContentType { get; set; }
        public DateTime GeneratedAt { get; set; }
        public string FileName { get; set; }
    }
}