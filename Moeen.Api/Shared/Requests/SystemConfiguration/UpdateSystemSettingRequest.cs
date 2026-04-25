using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.SystemConfiguration
{
    public class UpdateSystemSettingRequest
    {
        [Required(ErrorMessage = "Setting key is required")]
        [StringLength(100)]
        public string Key { get; set; } = string.Empty;

        [Required(ErrorMessage = "Setting value is required")]
        [StringLength(1000)]
        public string Value { get; set; } = string.Empty;
    }
}