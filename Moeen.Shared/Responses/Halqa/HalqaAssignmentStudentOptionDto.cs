using System;

namespace Moeen.Shared.Responses.Halqa
{
    public class HalqaAssignmentStudentOptionDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsSelected { get; set; }
        public Guid? HalqaId { get; set; }
        public string HalqaName { get; set; } = string.Empty;
    }
}
