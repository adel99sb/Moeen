using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.SystemConfiguration
{
    public class TimingDto
    {
        [Required(ErrorMessage = "Start time is required")]
        public TimeSpan StartTime { get; set; }

        [Required(ErrorMessage = "End time is required")]
        public TimeSpan EndTime { get; set; }

        [StringLength(100, ErrorMessage = "Timezone cannot exceed 100 characters")]
        public string Timezone { get; set; }

        public bool IsActive { get; set; }
    }
}