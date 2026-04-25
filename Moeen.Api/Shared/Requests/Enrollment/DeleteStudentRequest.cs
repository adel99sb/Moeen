using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.Enrollment
{
    public class DeleteStudentRequest
    {
        [Required(ErrorMessage = "Student ID is required")]
        public Guid StudentId { get; set; }
    }
}