using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.Goal
{
    public class SetDailyGoalRequest
    {
        [Required(ErrorMessage = "Student ID is required")]
        public Guid StudentId { get; set; }

        [Required(ErrorMessage = "Daily goal is required")]
        public DailyGoalDto Goal { get; set; }
    }
}