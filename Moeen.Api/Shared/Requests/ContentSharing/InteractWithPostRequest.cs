using Moeen.Api.Core.Constants;
using Moeen.Api.Core.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.ContentSharing
{
    public class InteractWithPostRequest
    {
        [Required(ErrorMessage = "Post ID is required")]
        public Guid PostId { get; set; }

        [Required(ErrorMessage = "Interaction type is required")]
        public InteractionType Type { get; set; }
    }
}