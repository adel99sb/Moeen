using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.ImportExport
{
    public class CancelJobRequest
    {
        [Required(ErrorMessage = "Job ID is required")]
        public Guid JobId { get; set; }

        [StringLength(500, ErrorMessage = "Reason cannot exceed 500 characters")]
        public string? Reason { get; set; }
    }
}