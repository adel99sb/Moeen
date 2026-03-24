using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.Reporting
{
    public class CompareReportsRequest
    {
        [Required(ErrorMessage = "Start date is required")]
        public DateTime Start { get; set; }

        [Required(ErrorMessage = "End date is required")]
        public DateTime End { get; set; }
    }
}