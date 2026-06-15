using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.ContentSharing
{
    public class RequstePostDto
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Title must be between 2 and 200 characters")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Body is required")]
        [StringLength(5000, ErrorMessage = "Body cannot exceed 5000 characters")]
        public string Body { get; set; } = string.Empty;

        [Url(ErrorMessage = "Invalid image URL")]
        public string? ImageUrl { get; set; }

        public Guid? HalqaId { get; set; }
    }
}
