using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.Reporting
{
    public class GetReportByIdRequest
    {
        [Required(ErrorMessage = "Report ID is required")]
        public Guid ReportId { get; set; }
    }
}