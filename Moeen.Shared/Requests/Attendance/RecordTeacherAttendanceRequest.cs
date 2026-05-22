using System;
using System.ComponentModel.DataAnnotations;
using Moeen.Shared.Constants;

namespace Moeen.Shared.Requests.Attendance
{
    public class RecordTeacherAttendanceRequest
    {
        public Guid? TeacherId { get; set; }
        public DateTime? Date { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }

        [Required(ErrorMessage = "Õ«·… «·Õ÷Ê— „ÿ·Ê»…")]
        public AttendanceStatus Status { get; set; }
    }
}