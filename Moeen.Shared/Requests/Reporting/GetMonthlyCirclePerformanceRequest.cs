using System;

namespace Moeen.Shared.Requests.Reporting
{
    public class GetMonthlyCirclePerformanceRequest
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public Guid? TeacherId { get; set; }
    }
}