using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.SystemConfiguration
{
    public class PeriodDto
    {
        [Required(ErrorMessage = "Period name is required")]
        [StringLength(100, ErrorMessage = "Period name cannot exceed 100 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Start date is required")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End date is required")]
        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; }
    }
}