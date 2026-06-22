using System;
using System.Collections.Generic;

namespace Moeen.Shared.Responses.Mobile
{
    public class ParentProfileResponse
    {
        public Guid ParentId { get; set; }
        public string ParentName { get; set; } = string.Empty;
        public string ParentInitials { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public DateTime? JoinedAt { get; set; }
        public int ChildrenCount { get; set; }
        public int TotalChildrenPoints { get; set; }
        public List<ParentProfileChildDto> Children { get; set; } = new();
    }

    public class ParentProfileChildDto
    {
        public Guid StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string StudentInitials { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string MosqueName { get; set; } = string.Empty;
        public string FoujName { get; set; } = string.Empty;
        public string HalqaName { get; set; } = string.Empty;
        public string TeacherName { get; set; } = string.Empty;
        public int TotalPoints { get; set; }
        public int Status { get; set; }
        public DateTime? EnrollmentDate { get; set; }
    }

    public class SubmitParentProfileNoteRequest
    {
        public string Content { get; set; } = string.Empty;
    }
}
