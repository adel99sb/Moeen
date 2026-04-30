using System;

namespace Moeen.Shared.Responses.ExamCommand
{
    public class ExamFeedbackDto
    {
        public Guid ExamId { get; set; }
        public string Feedback { get; set; } = string.Empty;
        public string? Recommendations { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}