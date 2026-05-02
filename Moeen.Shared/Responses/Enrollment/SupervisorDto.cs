using System;

namespace Moeen.Shared.Responses.Enrollment
{
    public class SupervisorDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public int Status { get; set; }
    }
}