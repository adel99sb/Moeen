using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.ContentSharing
{
    public class GetPostByIdRequest
    {
        [Required(ErrorMessage = "Post ID is required")]
        public Guid PostId { get; set; }

        /// <summary>
        ///  ÷„Ì‰  ›«’Ì· «· ›«⁄·«  ÷„‰ ‰ ÌÃ… «·„‰‘Ê—
        /// </summary>
        public bool IncludeInteractions { get; set; } = true;
    }
}