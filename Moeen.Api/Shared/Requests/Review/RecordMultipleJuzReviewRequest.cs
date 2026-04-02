using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.Review
{
    public class RecordMultipleJuzReviewRequest
    {
        [Required(ErrorMessage = "Student ID is required")]
        public Guid StudentId { get; set; }

        [Required(ErrorMessage = "Juz reviews list is required")]
        [MinLength(1, ErrorMessage = "At least one juz review must be provided")]
        public List<JuzReviewInputDto> JuzReviews { get; set; }
    }
}