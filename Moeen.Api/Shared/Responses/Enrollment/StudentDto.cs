using System;
using System.Collections.Generic;

namespace Moeen.Api.Shared.Responses.Enrollment
{
    public class StudentDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Gender { get; set; }
        public int FontSize { get; set; }
        public int Role { get; set; } // Should be 2 for Student
        public string Theme { get; set; }
        public string ProfileImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime JoinedAt { get; set; }

        // Student specific
        public int Age { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public int Status { get; set; }
        public int Score { get; set; }
        public Guid MosqueId { get; set; }
        public Guid? SaturdayHalqeId { get; set; }

        // Navigation properties (simplified)
        public string MosqueName { get; set; }
        public string SaturdayHalqeName { get; set; }
    }
}