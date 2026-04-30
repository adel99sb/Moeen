using System;

namespace Moeen.Shared.Requests.Enrollment
{
    public class GetMemberStatisticsRequest
    {
        public Guid? MosqueId { get; set; }
        public int? Status { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}