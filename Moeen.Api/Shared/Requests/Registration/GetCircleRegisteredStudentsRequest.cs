using Moeen.Api.Core.Constants;
using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.Registration
{
    public class GetCircleRegisteredStudentsRequest
    {
        [Required(ErrorMessage = "Circle ID is required")]
        public Guid CircleId { get; set; }

        public RegistrationStatus? Status { get; set; }
        public string? StudentName { get; set; }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}