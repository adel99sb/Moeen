using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.Attendance
{
    public class RecordDailyAttendanceRequest
    {
        [Required(ErrorMessage = "Entries list is required")]
        [MinLength(1, ErrorMessage = "At least one attendance entry is required")]
        public List<AttendanceEntry> Entries { get; set; }
    }
}