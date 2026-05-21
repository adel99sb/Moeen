using Moeen.Shared.Requests.Reporting;
using Moeen.Shared.Responses;
using System.Threading.Tasks;

namespace Moeen.Api.Core.Contracts.Application
{
    public interface IReportingService
    {
        /// <summary>
        /// [GET] مؤشرات الأداء العام
        /// </summary>
        Task<GeneralResponse> GetGeneralPerformanceIndicatorsAsync(GetGeneralPerformanceIndicatorsRequest request);

        /// <summary>
        /// [GET] أداء الحلقات شهريًا
        /// </summary>
        Task<GeneralResponse> GetMonthlyCirclePerformanceAsync(GetMonthlyCirclePerformanceRequest request);

        /// <summary>
        /// [GET] سجل التقدم التفصيلي لطالب
        /// </summary>
        Task<GeneralResponse> GetStudentProgressTimelineAsync(GetStudentProgressTimelineRequest request);
    }
}