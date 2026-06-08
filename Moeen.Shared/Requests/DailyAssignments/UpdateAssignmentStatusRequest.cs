using System;
using System.ComponentModel.DataAnnotations;
using Moeen.Shared.Constants;

namespace Moeen.Shared.Requests.DailyAssignments
{
    public class UpdateAssignmentStatusRequest
    {
        [Required(ErrorMessage = "Assignment ID is required")]
        public Guid AssignmentId { get; set; }

        [Required(ErrorMessage = "Assignment type is required")]
        public ProgressRecordType AssignmentType { get; set; }

        [Required(ErrorMessage = "Status is required")]
        public ReviewType Status { get; set; }

        public Grade? Grade { get; set; }
        public string? Notes { get; set; }
    }
}