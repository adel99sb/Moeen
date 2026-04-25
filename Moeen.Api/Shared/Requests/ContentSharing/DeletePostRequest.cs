using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.ContentSharing
{
    public class DeletePostRequest
    {
        [Required(ErrorMessage = "Post ID is required")]
        public Guid PostId { get; set; }
    }
}