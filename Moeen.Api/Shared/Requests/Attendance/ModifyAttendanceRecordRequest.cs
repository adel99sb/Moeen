using Moeen.Api.Core.Constants;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.Attendance
{
    public class ModifyAttendanceRecordRequest
    {
        [Required(ErrorMessage = "Record ID is required")]
        public Guid RecordId { get; set; }

        [Required(ErrorMessage = "New status is required")]
        public AttendanceStatus NewStatus { get; set; }
    }
}