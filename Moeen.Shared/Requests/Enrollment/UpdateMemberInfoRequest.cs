using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.Enrollment
{
    public class UpdateMemberInfoRequest
    {
        [Required(ErrorMessage = "Member ID is required")]
        public string MemberId { get; set; }

        // Optional fields that can be updated
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
        public string? Name { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string? Email { get; set; }

        [Phone(ErrorMessage = "Invalid phone number")]
        public string? Phone { get; set; }

        [RegularExpression("^(Male|Female)$", ErrorMessage = "Gender must be Male or Female")]
        public string? Gender { get; set; }

        [Range(8, 72, ErrorMessage = "Font size must be between 8 and 72")]
        public int? FontSize { get; set; }

        public string? Theme { get; set; }

        [Url(ErrorMessage = "Invalid image URL")]
        public string? ProfileImageUrl { get; set; }

        // Student specific
        [Range(1, 100, ErrorMessage = "Age must be between 1 and 100")]
        public int? Age { get; set; }

        [Range(0, 2, ErrorMessage = "Status must be between 0 and 2")]
        public int? Status { get; set; }

        [Range(0, 100, ErrorMessage = "Score must be between 0 and 100")]
        public int? Score { get; set; }

        public Guid? SaturdayHalqeId { get; set; }

        // Teacher specific
        [StringLength(500, ErrorMessage = "Bio cannot exceed 500 characters")]
        public string? Bio { get; set; }

        public string? AssignedAt { get; set; }

        // Parent specific
        public Guid? StudentId { get; set; }

        [StringLength(50, ErrorMessage = "Relationship cannot exceed 50 characters")]
        public string? Relationship { get; set; }
    }
}
