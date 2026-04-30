using Moeen.Shared.Constants;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.SystemConfiguration
{
    public class IssueUserLicenseRequest
    {
        [Required(ErrorMessage = "User ID is required")]
        public Guid UserId { get; set; }

        [Required(ErrorMessage = "License type is required")]
        public LicenseType Type { get; set; }
    }
}