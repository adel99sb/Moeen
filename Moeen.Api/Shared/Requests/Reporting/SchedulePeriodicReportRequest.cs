using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.Reporting
{
    public class SchedulePeriodicReportRequest
    {
        [Required(ErrorMessage = "Schedule data is required")]
        public ReportingScheduleDto Schedule { get; set; }
    }
}