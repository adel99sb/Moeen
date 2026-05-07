using System;

namespace Moeen.Shared.Responses.Authorization
{
    public class AccountDetailsDto
    {
        public Guid UserId { get; set; }
        public string Role { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string Gender { get; set; } = string.Empty;
        public DateTime? AssignedAt { get; set; }
        public Guid? MosqueId { get; set; }
        public Guid? FoujId { get; set; }
        public bool IsActive { get; set; }

        // ··√Â«·Ì
        public Guid? StudentId { get; set; }
        public string? Relationship { get; set; }
    }
}