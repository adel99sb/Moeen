using System;
using System.Collections.Generic;

namespace Moeen.Api.Shared.Responses.Enrollment
{
    public class MemberProfileDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Gender { get; set; }
        public string MemberType { get; set; }
        public int Role { get; set; }
        public string ProfileImageUrl { get; set; }
        public DateTime JoinedAt { get; set; }
        public int Status { get; set; }
        public Guid? MosqueId { get; set; }
        public string MosqueName { get; set; }

        // Additional profile details depending on type
        public StudentProfileDetails StudentDetails { get; set; }
        public TeacherProfileDetails TeacherDetails { get; set; }
        public ParentProfileDetails ParentDetails { get; set; }
    }

    public class StudentProfileDetails
    {
        public int Age { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public int Score { get; set; }
        public Guid? SaturdayHalqeId { get; set; }
        public string SaturdayHalqeName { get; set; }
        public int ProgressCount { get; set; }
        public int ExamCount { get; set; }
    }

    public class TeacherProfileDetails
    {
        public string Bio { get; set; }
        public string AssignedAt { get; set; }
        public int HalaqasCount { get; set; }
    }

    public class ParentProfileDetails
    {
        public Guid StudentId { get; set; }
        public string StudentName { get; set; }
        public string Relationship { get; set; }
    }
}