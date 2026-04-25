using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.ExamCommand
{
    public class AddExamFeedbackRequest
    {
        [Required(ErrorMessage = "Exam ID is required")]
        public Guid ExamId { get; set; }

        [Required(ErrorMessage = "Feedback is required")]
        [StringLength(1000, MinimumLength = 3, ErrorMessage = "Feedback must be between 3 and 1000 characters")]
        public string Feedback { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Recommendations cannot exceed 500 characters")]
        public string? Recommendations { get; set; }
    }
}