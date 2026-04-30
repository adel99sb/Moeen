using Moeen.Shared.Constants;
using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.ContentSharing
{
    public class GetPostInteractionsRequest
    {
        [Required(ErrorMessage = "Post ID is required")]
        public Guid PostId { get; set; }

        /// <summary>
        /// İáÊÑÉ ÍÓÈ äæÚ ÇáÊİÇÚá (ÇÎÊíÇÑí)
        /// </summary>
        public InteractionType? Type { get; set; }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}