using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.Feedback
{
    public class SuggestionDto
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Title must be between 2 and 200 characters")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Content is required")]
        [StringLength(1000, MinimumLength = 5, ErrorMessage = "Content must be between 5 and 1000 characters")]
        public string Content { get; set; }

        [Required(ErrorMessage = "User ID is required")]
        public Guid UserId { get; set; }
    }
}