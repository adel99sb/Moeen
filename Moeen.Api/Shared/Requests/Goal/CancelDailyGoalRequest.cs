using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.Goal
{
    public class CancelDailyGoalRequest
    {
        [Required(ErrorMessage = "Goal ID is required")]
        public Guid GoalId { get; set; }

        [StringLength(500, ErrorMessage = "Reason cannot exceed 500 characters")]
        public string? Reason { get; set; }
    }
}