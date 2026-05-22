using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.Attendance
{
    public class GetTeacherAttendanceRateRequest
    {
        [Required(ErrorMessage = "укбн Чсуксу уисцШ")]
        public Guid TeacherId { get; set; }

        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}