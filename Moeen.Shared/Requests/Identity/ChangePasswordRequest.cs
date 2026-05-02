using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.Identity
{
    public class ChangePasswordRequest
    {
        public Guid UserId { get; set; }

        [Required(ErrorMessage = "New password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "New password must be at least 6 characters")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
    }
}