using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.HalqaQuery
{
    public class GetHalqaAttendanceReportRequest
    {
        [Required(ErrorMessage = "Halqa ID is required")]
        public Guid HalqaId { get; set; }

        [Required(ErrorMessage = "From date is required")]
        public DateTime FromDate { get; set; }

        [Required(ErrorMessage = "To date is required")]
        public DateTime ToDate { get; set; }
    }
}