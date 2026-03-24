using Moeen.Api.Core.Constants;
//using Moeen.Api.Core.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.Review
{
    public class RecordJuzReviewRequest
    {
        [Required(ErrorMessage = "Student ID is required")]
        public Guid StudentId { get; set; }

        [Required(ErrorMessage = "Juz number is required")]
        [Range(1, 30, ErrorMessage = "Juz number must be between 1 and 30")]
        public int JuzNumber { get; set; }

        [Required(ErrorMessage = "Overall grade is required")]
        public Grade OverallGrade { get; set; } 

        [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
        public string Notes { get; set; }
    }
}