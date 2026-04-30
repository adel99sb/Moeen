using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.CircleQuery
{
    public class GetCircleStatisticsRequest
    {
        [Required(ErrorMessage = "Circle ID is required")]
        public Guid CircleId { get; set; }

        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}