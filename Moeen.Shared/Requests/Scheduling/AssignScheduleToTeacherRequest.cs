using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.Scheduling
{
    public class AssignScheduleToTeacherRequest
    {
        [Required(ErrorMessage = "Teacher ID is required")]
        public Guid TeacherId { get; set; }

        [Required(ErrorMessage = "Schedule ID is required")]
        public Guid ScheduleId { get; set; }
    }
}