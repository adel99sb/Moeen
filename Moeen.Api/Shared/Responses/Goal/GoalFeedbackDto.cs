using System;

namespace Moeen.Api.Shared.Responses.Goal
{
    public class GoalFeedbackDto
    {
        public Guid GoalId { get; set; }
        public Guid TeacherId { get; set; }
        public string Feedback { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}