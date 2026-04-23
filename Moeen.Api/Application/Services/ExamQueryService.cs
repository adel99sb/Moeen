using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Shared.Requests.ExamQuery;
using Moeen.Api.Shared.Responses;
using Moeen.Api.Shared.Responses.ExamCommand;
using Moeen.Api.Shared.Responses.ExamQuery;

namespace Moeen.Api.Application.Services
{
    public class ExamQueryService : IExamQueryService
    {
        public Task<CircleComparisonDto> CompareCirclesPerformanceAsync(CompareCirclesRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<CircleExamAnalyticsDto> GetCircleExamAnalyticsAsync(GetCircleAnalyticsRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<ExamResultDto> GetExamResultByIdAsync(GetExamResultByIdRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<PagedList<ExamResultDto>> GetExamsByCircleAsync(GetExamsByCircleRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<List<ExamResultDto>> GetExamsByPhaseAsync(GetExamsByPhaseRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<List<ExamResultDto>> GetExamsByTeacherAsync(GetExamsByTeacherRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<ExamStatisticsDto> GetExamStatisticsAsync(GetExamStatisticsRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<GetStudentExamsResponse> GetStudentExamsAsync(GetStudentExamsRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<List<ExamResultDto>> GetStudentExamsByDateRangeAsync(GetStudentExamsByDateRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<ExportExamDataDto> PrepareExamDataForExportAsync(PrepareExportRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<SearchExamResultsResponse> SearchExamResultsAsync(SearchExamResultsRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
