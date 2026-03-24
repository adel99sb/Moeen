using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.Attendance
{
    public class MonitorFrequentAbsencesRequest
    {
        [Required(ErrorMessage = "Threshold is required")]
        [Range(1, 100, ErrorMessage = "Threshold must be between 1 and 100")]
        public int Threshold { get; set; }
    }
}