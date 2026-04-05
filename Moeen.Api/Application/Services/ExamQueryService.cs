using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Shared.Requests.ExamQuery;
using Moeen.Api.Shared.Responses.ExamCommand;
using Moeen.Api.Shared.Responses.ExamQuery;

namespace Moeen.Api.Application.Services
{
    public class ExamQueryService : IExamQueryService
    {
        public Task<ExamResultDto> GetExamResultByIdAsync(GetExamResultByIdRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<GetStudentExamsResponse> GetStudentExamsAsync(GetStudentExamsRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<SearchExamResultsResponse> SearchExamResultsAsync(SearchExamResultsRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
