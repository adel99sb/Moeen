using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.Enrollment
{
    public class UpdateStudentInfoRequest
    {
        [Required(ErrorMessage = "Student ID is required")]
        public Guid StudentId { get; set; }

        public int? Status { get; set; }
        public int? Score { get; set; }
        public string? Notes { get; set; }
    }
}