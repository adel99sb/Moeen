using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.HalqaTeacherAssignment
{
    public class AssignTeacherToHalqaRequest
    {
        [Required(ErrorMessage = "Halqa ID is required")]
        public Guid HalqaId { get; set; }

        [Required(ErrorMessage = "Teacher ID is required")]
        public Guid TeacherId { get; set; }

        public bool IsPrimary { get; set; } = false;
        
    }
}