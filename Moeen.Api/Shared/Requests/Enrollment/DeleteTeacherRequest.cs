using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.Enrollment
{
    public class DeleteTeacherRequest
    {
        [Required(ErrorMessage = "Teacher ID is required")]
        public Guid TeacherId { get; set; }
    }
}