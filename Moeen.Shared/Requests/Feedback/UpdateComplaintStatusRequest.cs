using System;
using System.ComponentModel.DataAnnotations;
using Moeen.Shared.Constants;

namespace Moeen.Shared.Requests.Feedback
{
    public class UpdateComplaintStatusRequest
    {
        [Required(ErrorMessage = "Complaint ID is required")]
        public Guid ComplaintId { get; set; }

        [Required(ErrorMessage = "Status is required")]
        public ComplaintStatus Status { get; set; }

        [StringLength(500, ErrorMessage = "Note cannot exceed 500 characters")]
        public string? Note { get; set; }
    }
}