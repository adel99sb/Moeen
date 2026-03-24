using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Shared.Requests.Review;
using Moeen.Api.Shared.Responses.Review;

namespace Moeen.Api.Application.Services
{
    public class ReviewService : IReviewService
    {
        public Task<ReviewResultDto> RecordJuzReviewAsync(RecordJuzReviewRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<MultipleJuzReviewResponse> RecordMultipleJuzReviewAsync(RecordMultipleJuzReviewRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<RecordReviewPageResponse> RecordReviewPageAsync(RecordReviewPageRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
