using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.ContentSharing
{
    public class AddMultimediaRequest
    {
        [Required(ErrorMessage = "Post ID is required")]
        public Guid PostId { get; set; }

        [Required(ErrorMessage = "Media URLs list is required")]
        [MinLength(1, ErrorMessage = "At least one media URL is required")]
        public List<string> MediaUrls { get; set; }
    }
}