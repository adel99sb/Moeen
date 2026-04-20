using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.Circle
{
    public class MoveCircleToFoujRequest
    {
        [Required(ErrorMessage = "Circle ID is required")]
        public Guid CircleId { get; set; }

        [Required(ErrorMessage = "Target fouj ID is required")]
        public Guid TargetFoujId { get; set; }
    }
}