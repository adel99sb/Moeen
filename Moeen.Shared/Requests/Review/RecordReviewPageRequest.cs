using Moeen.Shared.Constants;


//using Moeen.Api.Core.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.Review
{
    public class RecordReviewPageRequest
    {
        [Required(ErrorMessage = "Student ID is required")]
        public Guid StudentId { get; set; }

        [Required(ErrorMessage = "Page number is required")]
        [Range(1, 604, ErrorMessage = "Page number must be between 1 and 604")]
        public int PageNumber { get; set; }

        [Required(ErrorMessage = "Grade is required")]
        public Grade Grade { get; set; }

        [Required(ErrorMessage = "Review type is required")]
        public ReviewType ReviewType { get; set; }
    }
}