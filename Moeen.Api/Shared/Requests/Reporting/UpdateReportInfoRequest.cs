using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.Reporting
{
    public class UpdateReportInfoRequest
    {
        [Required(ErrorMessage = "Report ID is required")]
        public Guid ReportId { get; set; }

        [StringLength(200, MinimumLength = 2, ErrorMessage = "Title must be between 2 and 200 characters")]
        public string? Title { get; set; }

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string? Description { get; set; }
    }
}