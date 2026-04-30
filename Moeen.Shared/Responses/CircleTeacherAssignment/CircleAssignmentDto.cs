using System;

namespace Moeen.Shared.Responses.CircleTeacherAssignment
{
    public class CircleAssignmentDto
    {
        public Guid CircleId { get; set; }
        public string CircleName { get; set; } = string.Empty;

        public Guid TeacherId { get; set; }
        public string TeacherName { get; set; } = string.Empty;

        public bool IsPrimary { get; set; }
        public bool IsActive { get; set; }

        public DateTime AssignedAt { get; set; }
    }
}