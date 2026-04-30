using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.Authorization
{
    public class UserRolesRequest
    {
        [Required(ErrorMessage = "User ID is required")]
        public string UserId { get; set; }
    }
}