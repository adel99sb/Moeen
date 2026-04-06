using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.Attendance
{
    public class CalculateAttendanceRateRequest
    {
        [Required(ErrorMessage = "Student ID is required")]
        public Guid StudentId { get; set; }
    }
}