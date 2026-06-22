using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.SystemConfiguration
{
    public class ManageSystemSettingsRequest
    {
        [Required(ErrorMessage = "Settings data is required")]
        public SettingsDto Settings { get; set; } = null!;
    }
}