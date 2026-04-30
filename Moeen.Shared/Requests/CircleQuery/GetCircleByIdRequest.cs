using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.CircleQuery
{
    public class GetCircleByIdRequest
    {
        [Required(ErrorMessage = "Circle ID is required")]
        public Guid CircleId { get; set; }
    }
}