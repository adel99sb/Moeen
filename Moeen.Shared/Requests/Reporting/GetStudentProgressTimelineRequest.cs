using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.Reporting
{
    public class GetStudentProgressTimelineRequest
    {
        [Required(ErrorMessage = "Student ID is required")]
        public Guid StudentId { get; set; }

        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? Type { get; set; } // Exam / Memorization / Review
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}