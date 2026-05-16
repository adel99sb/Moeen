using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.Identity
{
    public class RegisterRequest
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Phone(ErrorMessage = "Invalid phone number")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        [RegularExpression("^(Male|Female)$", ErrorMessage = "Gender must be Male or Female")]
        public string Gender { get; set; }
        // أضيفي هذه الحقول الجديدة لكي يتعرف عليها الفورم
        public string Address { get; set; }
        public string Musqe { get; set; }
        public string Fouj { get; set; }
        public string Halaqa { get; set; }
        public string HaltHesab { get; set; }
        public string NameParent { get; set; }
        public string PhoneNumberParent { get; set; }
        public DateTime? Date { get; set; }
        public DateTime? Birthday { get; set; }
    
}
}