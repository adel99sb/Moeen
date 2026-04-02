using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.SystemConfiguration
{
    public class ConfigureTimingsRequest
    {
        [Required(ErrorMessage = "Timings data is required")]
        public TimingDto Timings { get; set; }
    }
}