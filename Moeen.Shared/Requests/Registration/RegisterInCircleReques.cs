using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.Registration
{
    public class RegisterInCircleRequest
    {
        [Required(ErrorMessage = "Student ID is required")]
        public Guid StudentId { get; set; }

        [Required(ErrorMessage = "Circle ID is required")]
        public Guid CircleId { get; set; }
    }
}