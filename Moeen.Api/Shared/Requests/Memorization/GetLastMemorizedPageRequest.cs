using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.Memorization
{
    public class GetLastMemorizedPageRequest
    {
        [Required(ErrorMessage = "Student ID is required")]
        public Guid StudentId { get; set; }
    }
}