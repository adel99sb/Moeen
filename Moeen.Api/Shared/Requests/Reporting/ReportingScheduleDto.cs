using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.Reporting
{
    public class ReportingScheduleDto
    {
        [Required(ErrorMessage = "Report type is required")]
        public string ReportType { get; set; } // "Attendance" or "Performance"

        [Required(ErrorMessage = "Cron expression is required")]
        [RegularExpression(@"^(\*|([0-9]|1[0-9]|2[0-9]|3[0-9]|4[0-9]|5[0-9])|\*\/[0-9]+)\s+(\*|([0-9]|1[0-9]|2[0-3])|\*\/[0-9]+)\s+(\*|([1-9]|1[0-9]|2[0-9]|3[0-1])|\*\/[0-9]+)\s+(\*|([1-9]|1[0-2])|\*\/[0-9]+)\s+(\*|([0-6])|\*\/[0-9]+)$",
            ErrorMessage = "Invalid cron expression")]
        public string CronExpression { get; set; }

        public string RecipientEmail { get; set; }
        public Guid? CircleId { get; set; } // للحضور
        public Guid? StudentId { get; set; } // للأداء
    }
}