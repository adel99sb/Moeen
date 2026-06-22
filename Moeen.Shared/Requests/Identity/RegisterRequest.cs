using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.Identity
{
    public class RegisterRequest
    {
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

        [Phone(ErrorMessage = "Invalid phone number")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Gender is required")]
        [RegularExpression("^(Male|Female)$", ErrorMessage = "Gender must be Male or Female")]
        public string Gender { get; set; } = string.Empty;
        // أضيفي هذه الحقول الجديدة لكي يتعرف عليها الفورم
        public string Address { get; set; } = string.Empty;
        public string Musqe { get; set; } = string.Empty;
        public string Fouj { get; set; } = string.Empty;
        public string Halaqa { get; set; } = string.Empty;
        public string HaltHesab { get; set; } = string.Empty;
        public string NameParent { get; set; } = string.Empty;
        public string PhoneNumberParent { get; set; } = string.Empty;
        public DateTime? Date { get; set; }
        public DateTime? Birthday { get; set; }
    
}
}