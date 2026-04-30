using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.SystemConfiguration
{
    public class GetTimePeriodByIdRequest
    {
        [Required(ErrorMessage = "Time period ID is required")]
        public Guid TimePeriodId { get; set; }
    }
}