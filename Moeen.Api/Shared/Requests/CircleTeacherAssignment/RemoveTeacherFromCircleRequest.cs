using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.CircleTeacherAssignment
{
    public class RemoveTeacherFromCircleRequest
    {
        [Required(ErrorMessage = "Circle ID is required")]
        public Guid CircleId { get; set; }

        [Required(ErrorMessage = "Teacher ID is required")]
        public Guid TeacherId { get; set; }
    }
}