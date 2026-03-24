using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.Points
{
    public class GetStudentPointsRequest
    {
        [Required(ErrorMessage = "Student ID is required")]
        public Guid StudentId { get; set; }
    }
}