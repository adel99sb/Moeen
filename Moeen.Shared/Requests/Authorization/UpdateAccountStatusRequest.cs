using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.Authorization
{
    public class UpdateAccountStatusRequest
    {
        [Required(ErrorMessage = "UserId is required")]
        public Guid UserId { get; set; }

        [Required(ErrorMessage = "Status is required")]
        public bool IsActive { get; set; }
    }
}