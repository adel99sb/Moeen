using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.Points
{
    public class GetPointsHistoryRequest
    {
        [Required(ErrorMessage = "Student ID is required")]
        public Guid StudentId { get; set; }
    }
}