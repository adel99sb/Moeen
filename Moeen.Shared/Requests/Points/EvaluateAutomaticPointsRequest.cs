using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.Points
{
    public class EvaluateAutomaticPointsRequest
    {
        [Required(ErrorMessage = "Student ID is required")]
        public Guid StudentId { get; set; }

        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}