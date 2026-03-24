using Moeen.Api.Shared.Requests.Analytics;
using Moeen.Api.Shared.Responses.Analytics;
using System.Threading.Tasks;

namespace Moeen.Api.Core.Contracts.Application
{
    public interface IAnalyticsService
    {
        /// <summary>
        /// تحليل بيانات طالب معين
        /// </summary>
        Task<StudentAnalyticsDto> AnalyzeStudentDataAsync(AnalyzeStudentDataRequest request);

        /// <summary>
        /// تحليل أداء معلم معين
        /// </summary>
        Task<TeacherAnalyticsDto> AnalyzeTeacherPerformanceAsync(AnalyzeTeacherPerformanceRequest request);

        /// <summary>
        /// تحليل فعالية حلقة معينة
        /// </summary>
        Task<CircleAnalyticsDto> AnalyzeCircleEffectivenessAsync(AnalyzeCircleEffectivenessRequest request);
    }
}