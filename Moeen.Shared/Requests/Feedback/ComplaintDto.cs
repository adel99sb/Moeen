using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.Feedback
{
    public class ComplaintDto
    {
        [Required(ErrorMessage = "Content is required")]
        [StringLength(1000, MinimumLength = 5, ErrorMessage = "Content must be between 5 and 1000 characters")]
        public string Content { get; set; } = string.Empty;

        [Required(ErrorMessage = "User ID is required")]
        public Guid UserId { get; set; }
    }
}