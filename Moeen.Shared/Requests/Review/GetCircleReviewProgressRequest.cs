using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.Review
{
    public class GetCircleReviewProgressRequest
    {
        [Required(ErrorMessage = "Circle ID is required")]
        public Guid CircleId { get; set; }
    }
}