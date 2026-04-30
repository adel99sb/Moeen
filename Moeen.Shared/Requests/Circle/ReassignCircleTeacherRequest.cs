using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.Circle
{
    public class ReassignCircleTeacherRequest
    {
        [Required(ErrorMessage = "Circle ID is required")]
        public Guid CircleId { get; set; }

        [Required(ErrorMessage = "New teacher ID is required")]
        public Guid NewTeacherId { get; set; }
    }
}