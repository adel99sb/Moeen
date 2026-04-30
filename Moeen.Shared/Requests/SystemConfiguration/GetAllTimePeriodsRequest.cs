using System;

namespace Moeen.Shared.Requests.SystemConfiguration
{
    public class GetAllTimePeriodsRequest
    {
        public bool? IsActive { get; set; }
        public string? Name { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}