using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.Scheduling
{
    public class GetCircleScheduleByIdRequest
    {
        [Required(ErrorMessage = "Schedule ID is required")]
        public Guid ScheduleId { get; set; }
    }
}