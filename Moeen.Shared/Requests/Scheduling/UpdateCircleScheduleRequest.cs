using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.Scheduling
{
    public class UpdateCircleScheduleRequest
    {
        [Required(ErrorMessage = "Schedule ID is required")]
        public Guid ScheduleId { get; set; }

        [Required(ErrorMessage = "Schedule data is required")]
        public ScheduleDto ScheduleData { get; set; } = new();
    }
}