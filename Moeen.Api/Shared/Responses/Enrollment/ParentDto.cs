using System;

namespace Moeen.Api.Shared.Responses.Enrollment
{
    public class ParentDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Gender { get; set; }
        public int FontSize { get; set; }
        public int Role { get; set; } // Should be 3 for Parent (assuming)
        public string Theme { get; set; }
        public string ProfileImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime JoinedAt { get; set; }

        // Parent specific
        public Guid StudentId { get; set; }
        public string Relationship { get; set; }

        // Navigation
        public string StudentName { get; set; }
    }
}