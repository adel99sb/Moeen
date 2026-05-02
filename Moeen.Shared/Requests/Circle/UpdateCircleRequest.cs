using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.Circle
{
    public class UpdateCircleRequest
    {
        [Required(ErrorMessage = "Circle ID is required")]
        public Guid CircleId { get; set; }

        [StringLength(100, MinimumLength = 2, ErrorMessage = "Circle name must be between 2 and 100 characters")]
        public string Name { get; set; }

        public Guid? FoujId { get; set; }

        public Guid? TeacherId { get; set; }

        [StringLength(50, ErrorMessage = "Circle type cannot exceed 50 characters")]
        public string Type { get; set; }
    }
}