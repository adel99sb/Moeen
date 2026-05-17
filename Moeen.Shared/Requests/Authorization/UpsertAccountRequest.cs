using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.Authorization
{
    public class UpsertAccountRequest
    {
        public Guid? UserId { get; set; }

        [Required(ErrorMessage = "Role is required")]
        public string Role { get; set; } = string.Empty;

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Invalid phone number")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        public string Gender { get; set; } = string.Empty;

        public string? Password { get; set; }
        public Guid? MosqueId { get; set; }
        public Guid? FoujId { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime? AssignedAt { get; set; }

        // ··√Â«·Ì («Œ Ì«—Ì)
        public Guid? StudentId { get; set; }
        public string? Relationship { get; set; }
    }
}