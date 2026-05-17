using System;

namespace Moeen.Shared.Responses.HalqaTeacherAssignment
{
    public class HalqaAssignmentDto
    {
        public Guid HalqaId { get; set; }
        public string HalqaName { get; set; } = string.Empty;

        public Guid TeacherId { get; set; }
        public string TeacherName { get; set; } = string.Empty;

        public bool IsPrimary { get; set; }
        public bool IsActive { get; set; }

        public DateTime AssignedAt { get; set; }
    }
}