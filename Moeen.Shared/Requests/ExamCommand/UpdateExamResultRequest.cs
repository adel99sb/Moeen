using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.ExamCommand
{
    public class UpdateExamResultRequest
    {
        [Required(ErrorMessage = "Exam ID is required")]
        public Guid ExamId { get; set; }

        [Required(ErrorMessage = "New score is required")]
        [Range(0, 100, ErrorMessage = "Score must be between 0 and 100")]
        public int NewScore { get; set; }

        [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
        public string Notes { get; set; }
    }
}