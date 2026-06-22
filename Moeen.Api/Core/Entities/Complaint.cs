using Moeen.Shared.Constants;

namespace Moeen.Api.Core.Entities
{
    public class Complaint
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string? Title { get; set; } // اختياري للاقتراحات
        public string content { get; set; } = string.Empty;
        public DateTime created_at { get; set; }
        public FeedbackType Type { get; set; } // لتمييز الشكوى من الاقتراح
        public ComplaintStatus? Status { get; set; } // للشكاوى
        public SuggestionStatus? SuggestionStatus { get; set; } // للاقتراحات
        public string? Response { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public User User { get; set; } = null!;
    }
}
