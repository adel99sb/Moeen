using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.Feedback
{
    public class SubmitSuggestionRequest
    {
        [Required(ErrorMessage = "Suggestion data is required")]
        public SuggestionDto SuggestionData { get; set; } = null!;
    }
}