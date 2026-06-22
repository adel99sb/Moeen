using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.Halqa
{
    public class CreateHalqaRequest
    {
        [Required(ErrorMessage = "Halqa name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Halqa name must be between 2 and 100 characters")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Fouj ID is required")]
        public Guid FoujId { get; set; }

        [Required(ErrorMessage = "Teacher ID is required")]
        public Guid TeacherId { get; set; }

        [Required(ErrorMessage = "Halqa type is required")]
        [StringLength(50, ErrorMessage = "Halqa type cannot exceed 50 characters")]
        public string Type { get; set; } = string.Empty;

        public List<Guid>? StudentIds { get; set; }
    }
}
