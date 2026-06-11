using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.User
{
    public class SendVerifyEmailCodeRequest
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [MaxLength(256, ErrorMessage = "Email cannot exceed 256 characters.")]
        public string email { get; set; }
    }
}
