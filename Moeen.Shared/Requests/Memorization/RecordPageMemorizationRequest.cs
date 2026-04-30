using Moeen.Shared.Constants;
using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.Memorization
{
    public class RecordPageMemorizationRequest
    {
        [Required(ErrorMessage = "Student ID is required")]
        public Guid StudentId { get; set; }

        [Required(ErrorMessage = "Page number is required")]
        [Range(1, 604, ErrorMessage = "Page number must be between 1 and 604")]
        public int PageNumber { get; set; }

        [Required(ErrorMessage = "Grade is required")]
        public Grade Grade { get; set; }  // استخدام enum

        [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
        public string Notes { get; set; }
    }
}