using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.DailyAssignments
{
    public class GetDailyAssignmentsRequest
    {
        [Required(ErrorMessage = "Student ID is required")]
        public Guid StudentId { get; set; }

        public DateTime? Date { get; set; }
        public Guid? HalqaId { get; set; }
    }
}