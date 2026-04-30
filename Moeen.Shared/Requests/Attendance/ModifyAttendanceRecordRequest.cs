using Moeen.Shared.Constants;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.Attendance
{
    public class ModifyAttendanceRecordRequest
    {
        [Required(ErrorMessage = "Record ID is required")]
        public Guid RecordId { get; set; }

        [Required(ErrorMessage = "New status is required")]
        public AttendanceStatus NewStatus { get; set; }
    }
}