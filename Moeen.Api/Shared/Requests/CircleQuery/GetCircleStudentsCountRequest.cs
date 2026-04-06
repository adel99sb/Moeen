using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.Circle
{
    public class GetCircleStudentsCountRequest
    {
        [Required(ErrorMessage = "Circle ID is required")]
        public Guid CircleId { get; set; }
    }
}