using System;

namespace Moeen.Shared.Requests.Memorization
{
    public class GetStrugglingStudentsRequest
    {
        public Guid? CircleId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int Limit { get; set; } = 5;
    }
}