using System;
using System.Collections.Generic;

namespace Moeen.Shared.Responses.Enrollment
{
    public class StudentDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public int FontSize { get; set; }
        public int Role { get; set; } // Should be 2 for Student
        public string Theme { get; set; } = string.Empty;
        public string ProfileImageUrl { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime JoinedAt { get; set; }

        // Student specific
        public int Age { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public int Status { get; set; }
        public int Score { get; set; }
        public Guid MosqueId { get; set; }
        public Guid? SaturdayHalqeId { get; set; }
        public Guid? HalqaId { get; set; }
        public string HalqaName { get; set; } = string.Empty;
        public Guid? FoujId { get; set; }
        public string FoujName { get; set; } = string.Empty;

        // Navigation properties (simplified)
        public string MosqueName { get; set; } = string.Empty;
        public string SaturdayHalqeName { get; set; } = string.Empty;
    }
}