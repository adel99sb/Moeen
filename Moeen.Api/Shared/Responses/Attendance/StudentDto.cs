using System;

namespace Moeen.Api.Shared.Responses.Attendance
{
    public class StudentDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }
}