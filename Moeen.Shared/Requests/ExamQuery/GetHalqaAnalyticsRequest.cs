using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.ExamQuery
{
    public class GetHalqaAnalyticsRequest
    {
        [Required(ErrorMessage = "Halqa ID is required")]
        public Guid HalqaId { get; set; }

        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}