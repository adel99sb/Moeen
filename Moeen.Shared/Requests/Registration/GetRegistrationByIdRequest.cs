using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.Registration
{
    public class GetRegistrationByIdRequest
    {
        [Required(ErrorMessage = "Registration ID is required")]
        public Guid RegistrationId { get; set; }
    }
}