using Moeen.Shared.Requests.Goal;            
using Moeen.Shared.Responses;                 
using System.Threading.Tasks;                  

namespace Moeen.Api.Core.Contracts.Application
{
    public interface IGoalService
    {
        // تسجيل إدخال يومي جديد
        Task<GeneralResponse> RecordDailyEntryAsync(RecordDailyEntryRequest request);

        // تحديث سجل تقدم موجود
        Task<GeneralResponse> UpdateProgressRecordAsync(UpdateProgressRecordRequest request);

        // جلب ملخص تقدم طالب
        Task<GeneralResponse> GetStudentProgressSummaryAsync(GetStudentProgressSummaryRequest request);

        // جلب سجل تقدم طالب التاريخي
        Task<GeneralResponse> GetStudentProgressHistoryAsync(GetStudentProgressHistoryRequest request);

        // جلب نظرة عامة على أداء حلقة
        Task<GeneralResponse> GetCirclePerformanceOverviewAsync(GetCirclePerformanceOverviewRequest request);
    }
}