using System.Collections.Generic;

namespace Moeen.Api.Shared.Responses.Attendance
{
    public class FrequentAbsencesResultDto
    {
        public int Threshold { get; set; }
        public int TotalStudents { get; set; }
        public List<AttendanceStudentDto> Students { get; set; } = new();
    }
}