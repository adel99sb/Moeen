using System;

namespace Moeen.Shared.Responses.ImportExport
{
    public class ImportExportJobStatusDto
    {
        public Guid JobId { get; set; }
        public string JobType { get; set; } = string.Empty; // Import / Export
        public string Status { get; set; } = string.Empty;  // Pending / Running / Completed / Failed / Canceled
        public string? Message { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? FinishedAt { get; set; }
    }
}