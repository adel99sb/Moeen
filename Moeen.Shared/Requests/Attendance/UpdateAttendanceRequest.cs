using Moeen.Shared.Constants;

namespace Moeen.Shared.Requests.Attendance
{
    public class UpdateAttendanceRequest
    {
        public AttendanceStatus Status { get; set; }
        public string? Note { get; set; }
    }
}