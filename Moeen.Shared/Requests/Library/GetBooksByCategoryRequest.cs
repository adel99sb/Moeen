using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.Library
{
    public class GetBooksByCategoryRequest
    {
        [Required(ErrorMessage = "Category is required")]
        [StringLength(100, ErrorMessage = "Category cannot exceed 100 characters")]
        public string Category { get; set; } = string.Empty;
    }
}