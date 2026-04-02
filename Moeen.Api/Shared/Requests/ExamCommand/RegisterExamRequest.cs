using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.ExamCommand
{
    public class RegisterExamRequest
    {
        [Required(ErrorMessage = "Student ID is required")]
        public Guid StudentId { get; set; }

        [Required(ErrorMessage = "Teacher ID is required")]
        public Guid TeacherId { get; set; }

        [Required(ErrorMessage = "Juz from is required")]
        [Range(1, 30, ErrorMessage = "Juz from must be between 1 and 30")]
        public int JuzFrom { get; set; }

        [Required(ErrorMessage = "Juz to is required")]
        [Range(1, 30, ErrorMessage = "Juz to must be between 1 and 30")]
        public int JuzTo { get; set; }

        [Range(0, 100, ErrorMessage = "Score must be between 0 and 100")]
        public int Score { get; set; }

        [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
        public string Notes { get; set; }

        [Range(0, 100, ErrorMessage = "Mark must be between 0 and 100")]
        public int Mark { get; set; }
    }
}