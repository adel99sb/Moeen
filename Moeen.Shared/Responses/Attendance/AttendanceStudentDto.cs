using System;

namespace Moeen.Shared.Responses.Attendance
{
    public class AttendanceStudentDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }
}