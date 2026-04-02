using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.Scheduling
{
    public class ScheduleDto
    {
        [Required(ErrorMessage = "Schedule name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
        public string Name { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Start time is required")]
        public TimeSpan StartTime { get; set; }

        [Required(ErrorMessage = "End time is required")]
        public TimeSpan EndTime { get; set; }

        [StringLength(50, ErrorMessage = "Days of week cannot exceed 50 characters")]
        public string DaysOfWeek { get; set; } // e.g., "Mon,Wed,Fri"

        public bool IsActive { get; set; } = true;
    }
}