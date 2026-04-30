using System;

namespace Moeen.Shared.Responses.Attendance
{
    public class AttendanceRateDto
    {
        public Guid StudentId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int TotalSessions { get; set; }
        public int PresentCount { get; set; }
        public int AbsentCount { get; set; }
        public double Rate { get; set; }
    }
}