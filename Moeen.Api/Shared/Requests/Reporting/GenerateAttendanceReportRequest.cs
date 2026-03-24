using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.Reporting
{
    public class GenerateAttendanceReportRequest
    {
        [Required(ErrorMessage = "Circle ID is required")]
        public Guid CircleId { get; set; }
    }
}