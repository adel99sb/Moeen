using Moeen.Shared.Constants;
using System;

namespace Moeen.Shared.Responses.Registration
{
    public class CircleStudentDto
    {
        public Guid StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public RegistrationStatus Status { get; set; }
        public DateTime RegisteredAt { get; set; }
    }
}