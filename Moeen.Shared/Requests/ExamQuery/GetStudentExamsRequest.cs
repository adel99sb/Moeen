using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.ExamQuery
{
    public class GetStudentExamsRequest
    {
        [Required(ErrorMessage = "Student ID is required")]
        public Guid StudentId { get; set; }
    }
}