using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.Enrollment
{
    public class AddTeacherRequest
    {
        // User properties
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Compare("Password", ErrorMessage = "Password and confirmation do not match")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Invalid phone number")]
        public string Phone { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Gender is required")]
        [RegularExpression("^(Male|Female)$", ErrorMessage = "Gender must be Male or Female")]
        public string Gender { get; set; } = string.Empty;

        public DateTime? DateOfBirth { get; set; }

        // Teacher specific
        [Required(ErrorMessage = "Mosque ID is required")]
        public Guid MosqueId { get; set; }

        [StringLength(500, ErrorMessage = "Bio cannot exceed 500 characters")]
        public string Bio { get; set; } = string.Empty; // was 'boi' in entity

        public string AssignedAt { get; set; } = string.Empty; // could be date string or DateTime
        public List<string>? Experiences { get; set; }      // مثلاً: "معلم قرآن منذ 10 سنوات"
        public List<string>? Achievements { get; set; }    // مثلاً: "تخريج 50 طالباً"
        public List<string>? TeachingPlaces { get; set; }  // مثلاً: "جامع الورق الكبير"
    }
}