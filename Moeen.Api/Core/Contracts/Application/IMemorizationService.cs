using Moeen.Shared.Requests.Memorization;   
using Moeen.Shared.Responses;               
using System.Threading.Tasks;               
namespace Moeen.Api.Core.Contracts.Application
{
    public interface IMemorizationService
    {
        // تسجيل حفظ صفحة جديدة
        Task<GeneralResponse> RecordNewPageMemorizationAsync(RecordPageMemorizationRequest request);

        // جلب آخر صفحة تم حفظها للطالب
        Task<GeneralResponse> GetLastMemorizedPageAsync(GetLastMemorizedPageRequest request);

        // جلب تفاصيل سجل حفظ محدد
        Task<GeneralResponse> GetMemorizationRecordAsync(GetMemorizationRecordRequest request);

        // جلب السجل التاريخي الكامل لحفظ الطالب
        Task<GeneralResponse> GetStudentMemorizationHistoryAsync(GetStudentMemorizationHistoryRequest request);

        // جلب إحصائيات عامة عن عملية الحفظ
        Task<GeneralResponse> GetMemorizationStatisticsAsync(GetMemorizationStatisticsRequest request);

        // جلب تقدم الحفظ على مستوى الحلقة الدراسية
        Task<GeneralResponse> GetCircleMemorizationProgressAsync(GetCircleProgressRequest request);

        // جلب تقرير مفصل وشامل عن تقدم الحفظ
        Task<GeneralResponse> GetMemorizationProgressReportAsync(GetProgressReportRequest request);

        // تحديث تقدير/درجة سجل حفظ موجود
        Task<GeneralResponse> UpdateMemorizationGradeAsync(UpdateMemorizationGradeRequest request);

        // حذف سجل حفظ من النظام
        Task<GeneralResponse> DeleteMemorizationRecordAsync(DeleteMemorizationRecordRequest request);
    }
}