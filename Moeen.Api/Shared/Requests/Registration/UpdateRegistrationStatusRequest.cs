using Moeen.Api.Core.Constants;
using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.Registration
{
    public class UpdateRegistrationStatusRequest
    {
        [Required(ErrorMessage = "Registration ID is required")]
        public Guid RegistrationId { get; set; }

        [Required(ErrorMessage = "Status is required")]
        public RegistrationStatus Status { get; set; }

        [StringLength(500, ErrorMessage = "Reason cannot exceed 500 characters")]
        public string? Reason { get; set; }
    }
}