using Moeen.Dashboard.Application.Services.Abstractions;
using Moeen.Dashboard.Infrastructure.Http.Clients;
using Moeen.Shared.Requests.ExamQuery;
using Moeen.Shared.Responses;

namespace Moeen.Dashboard.Application.Services.Implementations
{
    public class ExamQueryService : IExamQueryService
    {
        private readonly ExamQueryApiClient _client;

        public ExamQueryService(ExamQueryApiClient client)
        {
            _client = client;
        }

        public async Task<GeneralResponse> GetExamResultByIdAsync(GetExamResultByIdRequest request)
        {
            var response = await _client.GetExamResultByIdAsync(request);
            return response;
        }

        public async Task<GeneralResponse> SearchExamResultsAsync(SearchExamResultsRequest request)
        {
            var response = await _client.SearchExamResultsAsync(request);
            return response;
        }

        public async Task<GeneralResponse> GetStudentExamsAsync(GetStudentExamsRequest request)
        {
            var response = await _client.GetStudentExamsAsync(request);
            return response;
        }

        public async Task<GeneralResponse> GetExamsByHalqaAsync(GetExamsByHalqaRequest request)
        {
            var response = await _client.GetExamsByHalqaAsync(request);
            return response;
        }

        public async Task<GeneralResponse> GetStudentExamsByDateRangeAsync(GetStudentExamsByDateRequest request)
        {
            var response = await _client.GetStudentExamsByDateRangeAsync(request);
            return response;
        }

        public async Task<GeneralResponse> GetExamsByTeacherAsync(GetExamsByTeacherRequest request)
        {
            var response = await _client.GetExamsByTeacherAsync(request);
            return response;
        }

        public async Task<GeneralResponse> GetExamsByPhaseAsync(GetExamsByPhaseRequest request)
        {
            var response = await _client.GetExamsByPhaseAsync(request);
            return response;
        }

        public async Task<GeneralResponse> GetExamStatisticsAsync(GetExamStatisticsRequest request)
        {
            var response = await _client.GetExamStatisticsAsync(request);
            return response;
        }

        public async Task<GeneralResponse> GetHalqaExamAnalyticsAsync(GetHalqaAnalyticsRequest request)
        {
            var response = await _client.GetHalqaExamAnalyticsAsync(request);
            return response;
        }

        public async Task<GeneralResponse> CompareHalqasPerformanceAsync(CompareHalqasRequest request)
        {
            var response = await _client.CompareHalqasPerformanceAsync(request);
            return response;
        }

        public async Task<GeneralResponse> PrepareExamDataForExportAsync(PrepareExportRequest request)
        {
            var response = await _client.PrepareExamDataForExportAsync(request);
            return response;
        }
    }
}