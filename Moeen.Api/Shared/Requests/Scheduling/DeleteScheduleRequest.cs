using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.Scheduling
{
    public class DeleteScheduleRequest
    {
        [Required(ErrorMessage = "Schedule ID is required")]
        public Guid ScheduleId { get; set; }
    }
}