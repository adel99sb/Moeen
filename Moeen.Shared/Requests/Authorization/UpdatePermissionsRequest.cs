using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.Authorization
{
    public class UpdatePermissionsRequest
    {
        [Required(ErrorMessage = "Permissions list is required")]
        [MinLength(1, ErrorMessage = "At least one permission is required")]
        public List<string> Permissions { get; set; } = new();
    }
}