using System;
using System.Collections.Generic;

namespace Moeen.Shared.Responses.Enrollment
{
    public class MemberProfileDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string MemberType { get; set; } = string.Empty;
        public int Role { get; set; }
        public string ProfileImageUrl { get; set; } = string.Empty;
        public DateTime JoinedAt { get; set; }
        public int Status { get; set; }
        public Guid? MosqueId { get; set; }
        public string MosqueName { get; set; } = string.Empty;

        // Additional profile details depending on type
        public StudentProfileDetails StudentDetails { get; set; } = null!;
        public TeacherProfileDetails TeacherDetails { get; set; } = null!;
        public ParentProfileDetails ParentDetails { get; set; } = null!;
    }

    public class StudentProfileDetails
    {
        public int Age { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public int Score { get; set; }
        public Guid? SaturdayHalqeId { get; set; }
        public string SaturdayHalqeName { get; set; } = string.Empty;
        public int ProgressCount { get; set; }
        public int ExamCount { get; set; }
    }

    public class TeacherProfileDetails
    {
        public string Bio { get; set; } = string.Empty;
        public string AssignedAt { get; set; } = string.Empty;
        public int HalaqasCount { get; set; }
    }

    public class ParentProfileDetails
    {
        public Guid StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string Relationship { get; set; } = string.Empty;
    }
}