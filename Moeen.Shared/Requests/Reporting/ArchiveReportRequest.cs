using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.Reporting
{
    public class ArchiveReportRequest
    {
        [Required(ErrorMessage = "Report ID is required")]
        public Guid ReportId { get; set; }
    }
}