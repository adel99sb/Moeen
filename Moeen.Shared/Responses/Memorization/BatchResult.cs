using System.Collections.Generic;

namespace Moeen.Shared.Responses.Memorization
{
    public class BatchResult
    {
        public int TotalPoints { get; set; }
        public int SuccessCount { get; set; }
        public int FailedCount { get; set; }
        public List<BatchItemResult> Items { get; set; } = new List<BatchItemResult>();
    }
}