using Moeen.Shared.Constants;
using System;

namespace Moeen.Shared.Responses.Registration
{
    public class RegistrationDto
    {
        public Guid RegistrationId { get; set; }
        public Guid StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public Guid CircleId { get; set; }
        public string CircleName { get; set; } = string.Empty;
        public RegistrationStatus Status { get; set; }
        public DateTime RegisteredAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}