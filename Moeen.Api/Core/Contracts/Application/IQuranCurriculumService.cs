using Moeen.Api.Shared.Requests.QuranCurriculum;
using Moeen.Api.Shared.Responses.QuranCurriculum;
using System.Threading.Tasks;

namespace Moeen.Api.Core.Contracts.Application
{
    public interface IQuranCurriculumService
    {
        /// <summary>
        /// تهيئة المصحف بأجزائه وصفحاته (تُستدعى مرة واحدة عند بدء النظام)
        /// </summary>
        /// <param name="request">طلب التهيئة (قد يكون فارغاً)</param>
        /// <returns>نتيجة العملية</returns>
        Task<InitializeQuranResponse> InitializeQuranAsync(InitializeQuranRequest request);

        /// <summary>
        /// الحصول على قائمة الأجزاء مع معلوماتها
        /// </summary>
        /// <param name="request">طلب القائمة (يدعم ترتيب وتصفية)</param>
        /// <returns>قائمة الأجزاء</returns>
        Task<GetJuzListResponse> GetJuzListAsync(GetJuzListRequest request);

        /// <summary>
        /// الحصول على الصفحات المكونة لجزء معين
        /// </summary>
        /// <param name="request">رقم الجزء</param>
        /// <returns>قائمة الصفحات</returns>
        Task<GetPagesByJuzResponse> GetPagesByJuzAsync(GetPagesByJuzRequest request);

        /// <summary>
        /// الحصول على الصفحات الجديدة المقترحة للطالب بناءً على آخر تقدم له
        /// </summary>
        /// <param name="request">معرف الطالب وعدد الصفحات المطلوبة</param>
        /// <returns>قائمة الصفحات المقترحة</returns>
        Task<GetNewMemorizationPagesResponse> GetNewMemorizationPagesAsync(GetNewMemorizationPagesRequest request);
    }
}