using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.Goal
{
    public class GetGoalByIdRequest
    {
        [Required(ErrorMessage = "Goal ID is required")]
        public Guid GoalId { get; set; }
    }
}