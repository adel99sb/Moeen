using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.Halqa
{
    public class UpdateHalqaRequest
    {
        [Required(ErrorMessage = "Halqa ID is required")]
        public Guid HalqaId { get; set; }

        [StringLength(100, MinimumLength = 2, ErrorMessage = "Halqa name must be between 2 and 100 characters")]
        public string Name { get; set; } = string.Empty;

        public Guid? FoujId { get; set; }

        public Guid? TeacherId { get; set; }

        [StringLength(50, ErrorMessage = "Halqa type cannot exceed 50 characters")]
        public string Type { get; set; } = string.Empty;

        public List<Guid>? StudentIds { get; set; }
    }
}
