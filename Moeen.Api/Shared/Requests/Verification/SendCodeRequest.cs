using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.Verification
{
    public class SendCodeRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string? Key { get; set; }
    }
}