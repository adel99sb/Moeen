using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.Goal
{
    public class GetGoalByIdRequest
    {
        [Required(ErrorMessage = "Goal ID is required")]
        public Guid GoalId { get; set; }
    }
}