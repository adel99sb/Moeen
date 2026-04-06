using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.SystemConfiguration
{
    public class ManageSystemSettingsRequest
    {
        [Required(ErrorMessage = "Settings data is required")]
        public SettingsDto Settings { get; set; }
    }
}