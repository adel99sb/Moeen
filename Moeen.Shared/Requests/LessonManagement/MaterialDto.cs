using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.LessonManagement
{
    public class MaterialDto
    {
        [Required(ErrorMessage = "Material title is required")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Material URL is required")]
        [Url(ErrorMessage = "Invalid URL")]
        public string Url { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "Description cannot exceed 200 characters")]
        public string Description { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty; // "PDF", "Video", "Link", etc.
    }
}