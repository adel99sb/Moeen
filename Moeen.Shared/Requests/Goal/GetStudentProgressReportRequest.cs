using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.Goal
{
    public class GetStudentProgressReportRequest
    {
        [Required(ErrorMessage = "Student ID is required")]
        public Guid StudentId { get; set; }

        [Required(ErrorMessage = "From date is required")]
        public DateTime FromDate { get; set; }

        [Required(ErrorMessage = "To date is required")]
        public DateTime ToDate { get; set; }
    }
}