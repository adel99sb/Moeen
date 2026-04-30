using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.Goal
{
    public class AddGoalFeedbackRequest
    {
        [Required(ErrorMessage = "Goal ID is required")]
        public Guid GoalId { get; set; }

        [Required(ErrorMessage = "Teacher ID is required")]
        public Guid TeacherId { get; set; }

        [Required(ErrorMessage = "Feedback is required")]
        [StringLength(1000, MinimumLength = 2, ErrorMessage = "Feedback must be between 2 and 1000 characters")]
        public string Feedback { get; set; } = string.Empty;
    }
}