using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.Goal
{
    public class UpdateDailyGoalRequest
    {
        [Required(ErrorMessage = "Goal ID is required")]
        public Guid GoalId { get; set; }

        [Required(ErrorMessage = "Student ID is required")]
        public Guid StudentId { get; set; }

        [Required(ErrorMessage = "Goal data is required")]
        public DailyGoalDto Goal { get; set; } = new();
    }
}