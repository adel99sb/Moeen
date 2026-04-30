using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.Circle
{
    public class DeleteCircleRequest
    {
        [Required(ErrorMessage = "Circle ID is required")]
        public Guid CircleId { get; set; }
    }
}