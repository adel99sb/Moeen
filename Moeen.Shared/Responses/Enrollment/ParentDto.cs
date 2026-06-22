using System;

namespace Moeen.Shared.Responses.Enrollment
{
    public class ParentDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public int FontSize { get; set; }
        public int Role { get; set; } // Should be 3 for Parent (assuming)
        public string Theme { get; set; } = string.Empty;
        public string ProfileImageUrl { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime JoinedAt { get; set; }

        // Parent specific
        public Guid StudentId { get; set; }
        public string Relationship { get; set; } = string.Empty;

        // Navigation
        public string StudentName { get; set; } = string.Empty;
    }
}