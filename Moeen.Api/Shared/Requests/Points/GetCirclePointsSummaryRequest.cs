using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.Points
{
    public class GetCirclePointsSummaryRequest
    {
        [Required(ErrorMessage = "Circle ID is required")]
        public Guid CircleId { get; set; }
    }
}