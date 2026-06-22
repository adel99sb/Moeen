namespace Moeen.Shared.Responses.Feedback
{
    public class SubmitSuggestionResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public Guid? SuggestionId { get; set; }
    }
}