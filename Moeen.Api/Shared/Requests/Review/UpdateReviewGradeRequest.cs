using Moeen.Api.Core.Constants;
using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.Review
{
    public class UpdateReviewGradeRequest
    {
        [Required(ErrorMessage = "Review record ID is required")]
        public Guid ReviewRecordId { get; set; }

        [Required(ErrorMessage = "New grade is required")]
        public Grade NewGrade { get; set; }

        [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
        public string? Notes { get; set; }
    }
}