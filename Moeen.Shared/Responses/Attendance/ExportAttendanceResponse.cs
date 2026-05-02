namespace Moeen.Shared.Responses.Attendance
{
    public class ExportAttendanceResponse
    {
        public byte[] FileContent { get; set; }
        public string ContentType { get; set; } // e.g., "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
        public string FileName { get; set; }
    }
}