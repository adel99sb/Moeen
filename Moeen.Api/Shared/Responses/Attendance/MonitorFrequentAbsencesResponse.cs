using System.Collections.Generic;

namespace Moeen.Api.Shared.Responses.Attendance
{
    public class MonitorFrequentAbsencesResponse
    {
        public List<AttendanceStudentDto> Students { get; set; }
    }
}