using Moeen.Shared.Constants;

namespace Moeen.Shared.Requests.Attendance
{
    public class CreateAttendanceRequest
    {
        public Guid StudentId { get; set; }
        public Guid MosqueId { get; set; }
        public DateTime AttendanceDate { get; set; }
        public AttendanceStatus Status { get; set; }
        public string? Note { get; set; }
    }
}