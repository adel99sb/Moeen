using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.Registration
{
    public class TransferStudentRequest
    {
        [Required(ErrorMessage = "Student ID is required")]
        public Guid StudentId { get; set; }

        [Required(ErrorMessage = "Source Circle ID is required")]
        public Guid FromCircleId { get; set; }

        [Required(ErrorMessage = "Target Circle ID is required")]
        public Guid ToCircleId { get; set; }
    }
}