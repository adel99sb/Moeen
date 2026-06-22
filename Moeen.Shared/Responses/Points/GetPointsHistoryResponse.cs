using System.Collections.Generic;

namespace Moeen.Shared.Responses.Points
{
    public class GetPointsHistoryResponse
    {
        public List<PointsTransactionDto> Transactions { get; set; } = new List<PointsTransactionDto>();
        public int TotalCount { get; set; }
    }
}