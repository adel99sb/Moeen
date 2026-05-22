using System;

namespace Moeen.Shared.Responses.Attendance
{
    public class TeacherAttendanceRateDto
    {
        public Guid TeacherId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int TotalDays { get; set; }
        public int PresentDays { get; set; }
        public int AbsentDays { get; set; }
        public double Rate { get; set; }
    }
}