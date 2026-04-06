namespace Moeen.Api.Shared.Responses.Attendance
{
    public class RecordDailyAttendanceResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int RecordsInserted { get; set; }
    }
}