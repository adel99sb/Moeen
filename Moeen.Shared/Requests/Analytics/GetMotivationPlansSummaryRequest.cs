using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.Analytics
{
    public class GetMotivationPlansSummaryRequest
    {
        [Required(ErrorMessage = "Teacher ID is required")]
        public Guid TeacherId { get; set; }

        public int PlanSize { get; set; } = 5;
    }
}