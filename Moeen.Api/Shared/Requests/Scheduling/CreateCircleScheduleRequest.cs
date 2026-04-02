using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.Scheduling
{
    public class CreateCircleScheduleRequest
    {
        [Required(ErrorMessage = "Schedule data is required")]
        public ScheduleDto ScheduleData { get; set; }
    }
}