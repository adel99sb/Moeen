using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.ContentSharing
{
    public class UpdatePostRequest
    {
        [Required(ErrorMessage = "Post ID is required")]
        public Guid PostId { get; set; }

        [StringLength(200, MinimumLength = 2, ErrorMessage = "Title must be between 2 and 200 characters")]
        public string? Title { get; set; }

        [StringLength(5000, ErrorMessage = "Body cannot exceed 5000 characters")]
        public string? Body { get; set; }

        [Url(ErrorMessage = "Invalid image URL")]
        public string? ImageUrl { get; set; }

        /// <summary>
        /// Õ«·… «·„‰‘Ê— („ƒ—‘› √Ê ·«)
        /// </summary>
        public bool? IsArchived { get; set; }
    }
}