using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.HalqaQuery
{
    public class GetHalqaStatisticsRequest
    {
        [Required(ErrorMessage = "Halqa ID is required")]
        public Guid HalqaId { get; set; }

        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}