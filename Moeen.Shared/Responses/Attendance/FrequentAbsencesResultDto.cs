using System.Collections.Generic;

namespace Moeen.Shared.Responses.Attendance
{
    public class FrequentAbsencesResultDto
    {
        public int Threshold { get; set; }
        public int TotalStudents { get; set; }
        public List<AttendanceStudentDto> Students { get; set; } = new();
    }
}