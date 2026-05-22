using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.Analytics
{
    public class GetMostRegressingStudentRequest
    {
        [Required(ErrorMessage = "Teacher ID is required")]
        public Guid TeacherId { get; set; }

        public DateTime? RecentFrom { get; set; }
        public DateTime? RecentTo { get; set; }
        public int WindowDays { get; set; } = 30;
    }
}