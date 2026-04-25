using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.Review
{
    public class GetReviewsByDateRangeRequest
    {
        [Required(ErrorMessage = "Student ID is required")]
        public Guid StudentId { get; set; }

        [Required(ErrorMessage = "From date is required")]
        public DateTime FromDate { get; set; }

        [Required(ErrorMessage = "To date is required")]
        public DateTime ToDate { get; set; }
    }
}