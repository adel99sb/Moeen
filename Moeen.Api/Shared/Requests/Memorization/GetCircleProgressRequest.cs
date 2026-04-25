using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.Memorization
{
    public class GetCircleProgressRequest
    {
        [Required(ErrorMessage = "Circle ID is required")]
        public Guid CircleId { get; set; }
    }
}