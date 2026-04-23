using Moeen.Api.Core.Constants;
using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.ExamCommand
{
    public class UpdateExamInfoRequest
    {
        [Required(ErrorMessage = "Exam ID is required")]
        public Guid ExamId { get; set; }

        public DateTime? ExamDate { get; set; }

        public ExamType? ExamType { get; set; }

        [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
        public string? Notes { get; set; }
    }
}