using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.Authorization
{
    public class AccountFilterRequest
    {
        [Required(ErrorMessage = "Role is required")]
        public string Role { get; set; } = string.Empty; // Supervisor / Teacher / Student / Parent

        public string? Keyword { get; set; }
        public Guid? MosqueId { get; set; }
        public Guid? FoujId { get; set; }
        public bool? IsActive { get; set; }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}