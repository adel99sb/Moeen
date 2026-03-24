namespace Moeen.Api.Shared.Responses.Feedback
{
    public class SubmitSuggestionResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public Guid? SuggestionId { get; set; }
    }
}