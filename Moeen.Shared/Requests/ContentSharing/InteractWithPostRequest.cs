using Moeen.Shared.Constants;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.ContentSharing
{
    public class InteractWithPostRequest
    {
        [Required(ErrorMessage = "Post ID is required")]
        public Guid PostId { get; set; }

        [Required(ErrorMessage = "Interaction type is required")]
        public InteractionType Type { get; set; }
    }
}