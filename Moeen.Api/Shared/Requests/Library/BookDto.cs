using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.Library
{
    public class BookDto
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "Title must be between 1 and 200 characters")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Author is required")]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "Author must be between 1 and 200 characters")]
        public string Author { get; set; }

        [StringLength(20, ErrorMessage = "ISBN cannot exceed 20 characters")]
        public string ISBN { get; set; }

        [StringLength(100, ErrorMessage = "Publisher cannot exceed 100 characters")]
        public string Publisher { get; set; }

        [Range(1000, 2100, ErrorMessage = "Publication year must be between 1000 and 2100")]
        public int? PublicationYear { get; set; }

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string Description { get; set; }

        [Url(ErrorMessage = "Invalid cover image URL")]
        public string CoverImageUrl { get; set; }
    }
}