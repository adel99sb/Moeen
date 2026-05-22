using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.Attendance
{
    public class GetStudentAbsenceReportRequest
    {
        [Required(ErrorMessage = "معرف الطالب مطلوب")]
        public Guid StudentId { get; set; }

        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        /// <summary>
        /// عدد الأيام المتتالية التي تعتبر تنبيهًا (افتراضي 3)
        /// </summary>
        public int ConsecutiveAlertThreshold { get; set; } = 3;
    }
}