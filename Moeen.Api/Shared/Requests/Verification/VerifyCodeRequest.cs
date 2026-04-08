using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.Verification
{
    public class VerifyCodeRequest
    {
        [Required]
        public string Key { get; set; } = string.Empty;

        [Required]
        public string Code { get; set; } = string.Empty;
    }
}