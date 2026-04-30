using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.Authorization
{
    public class CheckAccessRequest
    {
        [Required(ErrorMessage = "User ID is required")]
        public string UserId { get; set; }

        [Required(ErrorMessage = "Permission is required")]
        [StringLength(100, ErrorMessage = "Permission cannot exceed 100 characters")]
        public string Permission { get; set; }
    }
}