using System;

namespace Moeen.Shared.Requests.Points
{
    public class GetLeaderboardRequest
    {
        public Guid? CircleId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}