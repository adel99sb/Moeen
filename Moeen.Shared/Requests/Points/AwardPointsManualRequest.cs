using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.Points
{
    public class AwardPointsManualRequest
    {
        [Required(ErrorMessage = "Student ID is required")]
        public Guid StudentId { get; set; }

        [Required(ErrorMessage = "Points is required")]
        [Range(1, 10000)]
        public int Points { get; set; }

        [Required(ErrorMessage = "Point type key is required")]
        [StringLength(50)]
        public string PointTypeKey { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Reason { get; set; }
    }
}