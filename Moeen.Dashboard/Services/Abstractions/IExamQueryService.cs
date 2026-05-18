using Moeen.Shared.Requests.ExamQuery;
using Moeen.Shared.Responses;

namespace Moeen.Dashboard.Application.Services.Abstractions
{
    public interface IExamQueryService
    {
        Task<GeneralResponse> GetExamResultByIdAsync(GetExamResultByIdRequest request);
        Task<GeneralResponse> SearchExamResultsAsync(SearchExamResultsRequest request);
        Task<GeneralResponse> GetStudentExamsAsync(GetStudentExamsRequest request);
        Task<GeneralResponse> GetExamsByHalqaAsync(GetExamsByHalqaRequest request);
        Task<GeneralResponse> GetStudentExamsByDateRangeAsync(GetStudentExamsByDateRequest request);
        Task<GeneralResponse> GetExamsByTeacherAsync(GetExamsByTeacherRequest request);
        Task<GeneralResponse> GetExamsByPhaseAsync(GetExamsByPhaseRequest request);
        Task<GeneralResponse> GetExamStatisticsAsync(GetExamStatisticsRequest request);
        Task<GeneralResponse> GetHalqaExamAnalyticsAsync(GetHalqaAnalyticsRequest request);
        Task<GeneralResponse> CompareHalqasPerformanceAsync(CompareHalqasRequest request);
        Task<GeneralResponse> PrepareExamDataForExportAsync(PrepareExportRequest request);
    }
}
