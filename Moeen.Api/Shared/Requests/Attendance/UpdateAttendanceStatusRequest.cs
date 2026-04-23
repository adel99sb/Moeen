using Moeen.Api.Core.Constants;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.Attendance
{
    public class UpdateAttendanceStatusRequest
    {
        [Required(ErrorMessage = "New status is required")]
        public AttendanceStatus NewStatus { get; set; }

        [StringLength(500, ErrorMessage = "Note cannot exceed 500 characters")]
        public string? Note { get; set; }
    }
}