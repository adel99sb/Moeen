using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.ContentSharing
{
    public class DeleteOldContentRequest
    {
        [Required(ErrorMessage = "Older than date is required")]
        public DateTime OlderThan { get; set; }
    }
}