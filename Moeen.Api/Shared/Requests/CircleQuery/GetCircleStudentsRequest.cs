using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.Circle
{
    public class GetCircleStudentsRequest
    {
        [Required(ErrorMessage = "Circle ID is required")]
        public Guid CircleId { get; set; }

        public StudentFilterDto Filter { get; set; } // يمكن أن يكون null
    }
}