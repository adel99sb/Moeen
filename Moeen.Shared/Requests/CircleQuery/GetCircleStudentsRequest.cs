using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.CircleQuery
{
    public class GetCircleStudentsRequest
    {
        [Required(ErrorMessage = "Circle ID is required")]
        public Guid CircleId { get; set; }

        public StudentFilterDto Filter { get; set; } // يمكن أن يكون null
    }
}