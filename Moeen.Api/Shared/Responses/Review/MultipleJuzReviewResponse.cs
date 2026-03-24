using System.Collections.Generic;

namespace Moeen.Api.Shared.Responses.Review
{
    public class MultipleJuzReviewResponse
    {
        public List<ReviewResultDto> Results { get; set; }
        public int TotalPoints { get; set; }
        public int SuccessCount { get; set; }
        public int FailedCount { get; set; }
    }
}