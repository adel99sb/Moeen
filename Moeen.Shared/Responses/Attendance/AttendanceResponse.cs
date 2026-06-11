using Moeen.Shared.Constants;

namespace Moeen.Shared.Responses.Attendance
{
    public class AttendanceResponse
    {
        public Guid Id { get; set; }
        public Guid StudentId { get; set; }
        public string StudentFullName { get; set; }
        public Guid MosqueId { get; set; }
        public string MosqueName { get; set; }
        public DateTime AttendanceDate { get; set; }
        public AttendanceStatus Status { get; set; }
        public string StatusName => Status.ToString();
        public string? Note { get; set; }
    }
}