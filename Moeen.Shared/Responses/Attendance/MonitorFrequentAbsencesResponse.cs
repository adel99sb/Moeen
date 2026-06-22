using System.Collections.Generic;

namespace Moeen.Shared.Responses.Attendance
{
    public class MonitorFrequentAbsencesResponse
    {
        public List<AttendanceStudentDto> Students { get; set; } = new List<AttendanceStudentDto>();
    }
}