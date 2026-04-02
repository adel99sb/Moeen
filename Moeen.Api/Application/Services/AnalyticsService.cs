using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Shared.Requests.Analytics;
using Moeen.Api.Shared.Responses.Analytics;

namespace Moeen.Api.Application.Services
{
    public class AnalyticsService : IAnalyticsService
    {
        public Task<CircleAnalyticsDto> AnalyzeCircleEffectivenessAsync(AnalyzeCircleEffectivenessRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<StudentAnalyticsDto> AnalyzeStudentDataAsync(AnalyzeStudentDataRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<TeacherAnalyticsDto> AnalyzeTeacherPerformanceAsync(AnalyzeTeacherPerformanceRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
