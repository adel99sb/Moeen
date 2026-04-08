using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.ContentSharing
{
    public class PublishPostRequest
    {
        [Required(ErrorMessage = "Post data is required")]
        public RequstePostDto PostData { get; set; }

        public bool IsPublic { get; set; } = true;
    }
}