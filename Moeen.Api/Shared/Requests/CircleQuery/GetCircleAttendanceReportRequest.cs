using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.CircleQuery
{
    public class GetCircleAttendanceReportRequest
    {
        [Required(ErrorMessage = "Circle ID is required")]
        public Guid CircleId { get; set; }

        [Required(ErrorMessage = "From date is required")]
        public DateTime FromDate { get; set; }

        [Required(ErrorMessage = "To date is required")]
        public DateTime ToDate { get; set; }
    }
}