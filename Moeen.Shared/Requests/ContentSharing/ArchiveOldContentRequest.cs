using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.ContentSharing
{
    public class ArchiveOldContentRequest
    {
        [Required(ErrorMessage = "Older than date is required")]
        public DateTime OlderThan { get; set; }
    }
}