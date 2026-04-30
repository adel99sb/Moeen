using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.ExamCommand
{
    public class DeleteExamResultRequest
    {
        [Required(ErrorMessage = "Exam ID is required")]
        public Guid ExamId { get; set; }
    }
}