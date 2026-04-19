using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Shared.Requests.Review;
using Moeen.Api.Shared.Responses;
using Moeen.Api.Shared.Responses.Review;

namespace Moeen.Api.Application.Services
{
    public class ReviewService : IReviewService
    {
        public Task<ReviewOperationResponse> DeleteReviewRecordAsync(DeleteReviewRecordRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<ReviewCertificateDto> GenerateReviewCertificateAsync(GenerateReviewCertificateRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<List<StudentReviewSummaryDto>> GetCircleReviewProgressAsync(GetCircleReviewProgressRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<ReviewRecordDto> GetReviewRecordByIdAsync(GetReviewRecordByIdRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<List<ReviewRecordDto>> GetReviewsByDateRangeAsync(GetReviewsByDateRangeRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<List<TeacherReviewSummaryDto>> GetReviewsByTeacherAsync(GetReviewsByTeacherRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<PagedList<ReviewRecordDto>> GetStudentReviewHistoryAsync(GetStudentReviewHistoryRequest request)
        {
            throw new NotImplementedException();
        }

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

        public Task<ReviewRecordDto> UpdateReviewGradeAsync(UpdateReviewGradeRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
