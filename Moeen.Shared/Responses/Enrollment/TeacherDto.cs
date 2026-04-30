using System;
using System.Collections.Generic;

namespace Moeen.Shared.Responses.Enrollment
{
    public class TeacherDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Gender { get; set; }
        public int FontSize { get; set; }
        public int Role { get; set; } // Should be 1 for Teacher
        public string Theme { get; set; }
        public string ProfileImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime JoinedAt { get; set; }

        // Teacher specific
        public Guid MosqueId { get; set; }
        public string Bio { get; set; }
        public string AssignedAt { get; set; }

        // Navigation
        public string MosqueName { get; set; }
    }
}