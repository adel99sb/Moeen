using Moeen.Api.Shared.Requests.Memorization;
using Moeen.Api.Shared.Responses.Memorization;
using System.Threading.Tasks;

namespace Moeen.Api.Core.Contracts.Application
{
    public interface IMemorizationService
    {
        /// <summary>
        /// تسجيل حفظ صفحة جديدة لطالب
        /// </summary>
        /// <param name="request">بيانات الصفحة المحفوظة</param>
        /// <returns>عدد النقاط المكتسبة</returns>
        Task<RecordPageMemorizationResponse> RecordNewPageMemorizationAsync(RecordPageMemorizationRequest request);

        /// <summary>
        /// تسجيل حفظ مجموعة صفحات دفعة واحدة
        /// </summary>
        /// <param name="request">معرف الطالب وقائمة الصفحات مع التقديرات</param>
        /// <returns>نتيجة العملية مع تفاصيل كل صفحة</returns>
        Task<BatchResult> RecordNewPagesBatchAsync(RecordPagesBatchRequest request);

        /// <summary>
        /// الحصول على آخر صفحة حفظها الطالب
        /// </summary>
        /// <param name="request">معرف الطالب</param>
        /// <returns>رقم آخر صفحة</returns>
        Task<GetLastMemorizedPageResponse> GetLastMemorizedPageAsync(GetLastMemorizedPageRequest request);
    }
}