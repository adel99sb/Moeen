using System;

namespace Moeen.Shared.Responses.Enrollment
{
    public class MemberDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Gender { get; set; }
        public string MemberType { get; set; } // "Student", "Teacher", "Parent", "Supervisor"
        public int Role { get; set; }
        public string ProfileImageUrl { get; set; }
        public DateTime JoinedAt { get; set; }
        public int Status { get; set; } // Common status
        public Guid? MosqueId { get; set; }
        public string MosqueName { get; set; }
    }
}