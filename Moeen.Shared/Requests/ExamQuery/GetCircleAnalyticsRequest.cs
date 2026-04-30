using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.ExamQuery
{
    public class GetCircleAnalyticsRequest
    {
        [Required(ErrorMessage = "Circle ID is required")]
        public Guid CircleId { get; set; }

        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}