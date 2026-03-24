using System.Collections.Generic;

namespace Moeen.Api.Shared.Responses.Points
{
    public class GetPointsHistoryResponse
    {
        public List<PointsTransactionDto> Transactions { get; set; }
        public int TotalCount { get; set; }
    }
}