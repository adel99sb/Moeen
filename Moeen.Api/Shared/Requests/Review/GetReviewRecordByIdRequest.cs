using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.Review
{
    public class GetReviewRecordByIdRequest
    {
        [Required(ErrorMessage = "Review record ID is required")]
        public Guid ReviewRecordId { get; set; }
    }
}