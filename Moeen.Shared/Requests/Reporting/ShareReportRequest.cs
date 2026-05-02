using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.Reporting
{
    public class ShareReportRequest
    {
        [Required(ErrorMessage = "Report ID is required")]
        public Guid ReportId { get; set; }

        [Required(ErrorMessage = "Recipient ID is required")]
        public Guid RecipientId { get; set; }
    }
}