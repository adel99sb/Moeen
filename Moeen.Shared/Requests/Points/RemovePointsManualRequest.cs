using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.Points
{
    public class RemovePointsManualRequest
    {
        [Required(ErrorMessage = "Student ID is required")]
        public Guid StudentId { get; set; }

        [Required(ErrorMessage = "Points is required")]
        [Range(1, 10000, ErrorMessage = "Points must be between 1 and 10000")]
        public int Points { get; set; }

        [StringLength(500)]
        public string? Reason { get; set; }
    }
}