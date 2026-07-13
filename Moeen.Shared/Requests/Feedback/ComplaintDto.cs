using System;
using System.ComponentModel.DataAnnotations;
using Moeen.Shared.Constants;

namespace Moeen.Shared.Requests.Feedback
{
    public class ComplaintDto
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Content is required")]
        [StringLength(1000, MinimumLength = 5, ErrorMessage = "Content must be between 5 and 1000 characters")]
        public string Content { get; set; } = string.Empty;

        [Required(ErrorMessage = "User ID is required")]
        public Guid UserId { get; set; }

        public string? Title { get; set; }
        public DateTime CreatedAt { get; set; }
        public FeedbackType Type { get; set; }
        public ComplaintStatus? Status { get; set; }
        public SuggestionStatus? SuggestionStatus { get; set; }
        public string? Response { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
