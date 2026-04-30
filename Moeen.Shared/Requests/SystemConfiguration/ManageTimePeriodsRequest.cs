using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.SystemConfiguration
{
    public class ManageTimePeriodsRequest
    {
        [Required(ErrorMessage = "Periods list is required")]
        [MinLength(1, ErrorMessage = "At least one period is required")]
        public List<PeriodDto> Periods { get; set; }
    }
}