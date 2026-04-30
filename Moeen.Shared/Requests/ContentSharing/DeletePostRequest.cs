using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.ContentSharing
{
    public class DeletePostRequest
    {
        [Required(ErrorMessage = "Post ID is required")]
        public Guid PostId { get; set; }
    }
}