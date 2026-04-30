using Moeen.Shared.Constants;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.Attendance
{
    public class AttendanceEntry
    {
        [Required(ErrorMessage = "Student ID is required")]
        public Guid StudentId { get; set; }

        [Required(ErrorMessage = "Date is required")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Status is required")]
        public AttendanceStatus Status { get; set; }

        [StringLength(500, ErrorMessage = "Note cannot exceed 500 characters")]
        public string Note { get; set; }
    }
}