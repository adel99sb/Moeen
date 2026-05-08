using System;
using System.ComponentModel.DataAnnotations;
using Moeen.Shared.Constants;

namespace Moeen.Shared.Requests.Feedback
{
    public class ManageFeedbackRequest
    {
        [Required(ErrorMessage = "Feedback ID is required")]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Response is required")]
        [StringLength(1000, MinimumLength = 1, ErrorMessage = "Response must be between 1 and 1000 characters")]
        public string Response { get; set; }

        [Required(ErrorMessage = "Feedback type is required")]
        public FeedbackType Type { get; set; }
    }
}