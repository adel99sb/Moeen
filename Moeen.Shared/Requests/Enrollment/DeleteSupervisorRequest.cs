using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.Enrollment
{
    public class DeleteSupervisorRequest
    {
        [Required(ErrorMessage = "Supervisor ID is required")]
        public Guid SupervisorId { get; set; }
    }
}
