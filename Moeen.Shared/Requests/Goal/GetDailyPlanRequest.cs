using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.Goal
{
    public class GetDailyPlanRequest
    {
        [Required(ErrorMessage = "Student ID is required")]
        public Guid StudentId { get; set; }

        [Required(ErrorMessage = "Date is required")]
        public DateTime Date { get; set; }
    }
}