using System.ComponentModel.DataAnnotations;
using Moeen.Shared.Constants;

namespace Moeen.Shared.Requests.Feedback
{
    public class UpdateSuggestionStatusRequest
    {
        [Required(ErrorMessage = "„⁄—› «·«ﬁ —«Õ „ÿ·Ê».")]
        public Guid SuggestionId { get; set; }

        [Required(ErrorMessage = "Õ«·… «·«ﬁ —«Õ „ÿ·Ê»….")]
        public SuggestionStatus Status { get; set; }
    }
}