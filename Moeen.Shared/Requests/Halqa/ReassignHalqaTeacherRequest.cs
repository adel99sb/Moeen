using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.Halqa
{
    public class ReassignHalqaTeacherRequest
    {
        [Required(ErrorMessage = "Halqa ID is required")]
        public Guid HalqaId { get; set; }

        [Required(ErrorMessage = "New teacher ID is required")]
        public Guid NewTeacherId { get; set; }
    }
}