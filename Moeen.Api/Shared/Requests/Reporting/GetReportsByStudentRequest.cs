using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.Reporting
{
    public class GetReportsByStudentRequest
    {
        [Required(ErrorMessage = "Student ID is required")]
        public Guid StudentId { get; set; }

        public string? Type { get; set; } // Attendance / Performance / Progress
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}