using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.User
{
    public class ResetPasswordRequest
    {
        [Required(ErrorMessage = "User ID is required")]
        public Guid Uid { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(8, ErrorMessage = "Passwords must be at least 8 characters.")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)[A-Za-z\d@$!%*?&]{8,}$",
            ErrorMessage = "Passwords must have at least one uppercase ('A'-'Z'), at least one digit ('0'-'9').")]
        [MaxLength(50, ErrorMessage = "Password cannot exceed 50 characters.")]
        public string NewPassword { get; set; }
    }
}
