using System;

namespace Moeen.Shared.Responses.Enrollment
{
    public class MemberDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string MemberType { get; set; } = string.Empty; // "Student", "Teacher", "Parent", "Supervisor"
        public int Role { get; set; }
        public string ProfileImageUrl { get; set; } = string.Empty;
        public DateTime JoinedAt { get; set; }
        public int Status { get; set; } // Common status
        public Guid? MosqueId { get; set; }
        public string MosqueName { get; set; } = string.Empty;
    }
}