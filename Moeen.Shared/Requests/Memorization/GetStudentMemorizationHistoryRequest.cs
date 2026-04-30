using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.Memorization
{
    public class GetStudentMemorizationHistoryRequest
    {
        [Required(ErrorMessage = "Student ID is required")]
        public Guid StudentId { get; set; }

        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}