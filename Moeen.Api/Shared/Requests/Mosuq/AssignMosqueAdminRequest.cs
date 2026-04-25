using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.Mosuq
{
    public class AssignMosqueAdminRequest
    {
        [Required(ErrorMessage = "Mosque ID is required")]
        public Guid MosqueId { get; set; }

        [Required(ErrorMessage = "Admin user ID is required")]
        public Guid AdminUserId { get; set; }
    }
}