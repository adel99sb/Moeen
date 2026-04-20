using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.SystemConfiguration
{
    public class UpdateTimePeriodRequest
    {
        [Required(ErrorMessage = "Time period ID is required")]
        public Guid TimePeriodId { get; set; }

        [StringLength(100)]
        public string? Name { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? IsActive { get; set; }
    }
}