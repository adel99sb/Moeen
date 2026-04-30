using Moeen.Shared.Requests.Mosuq;
using Moeen.Shared.Responses;
using System.Threading.Tasks;

namespace Moeen.Api.Core.Contracts.Application
{
    public interface IMosquService
    {
        Task<GeneralResponse> AddMosqu(AddMosquReq req);
        Task<GeneralResponse> GetAllMosqus(GetAllMosqusRequest request);

        /// <summary>
        /// [GET] جلب تفاصيل مسجد محدد بمعرفه مع المعلومات الكاملة
        /// </summary>
        Task<GeneralResponse> GetMosqueByIdAsync(GetMosqueByIdRequest request);

        /// <summary>
        /// [GET] جلب الحلقات الدراسية التابعة لمسجد معين
        /// </summary>
        Task<GeneralResponse> GetCirclesByMosqueAsync(GetCirclesByMosqueRequest request);

        /// <summary>
        /// [GET] جلب المعلمين المعينين في مسجد معين
        /// </summary>
        Task<GeneralResponse> GetTeachersByMosqueAsync(GetTeachersByMosqueRequest request);

        /// <summary>
        /// [GET] الحصول على إحصائيات مسجد
        /// </summary>
        Task<GeneralResponse> GetMosqueStatisticsAsync(GetMosqueStatisticsRequest request);

        /// <summary>
        /// [GET] جلب المساجد القريبة من موقع المستخدم الحالي
        /// </summary>
        Task<GeneralResponse> GetNearbyMosquesAsync(GetNearbyMosquesRequest request);

        /// <summary>
        /// [PUT] تحديث معلومات مسجد أساسي
        /// </summary>
        Task<GeneralResponse> UpdateMosqueInfoAsync(UpdateMosqueInfoRequest request);

        /// <summary>
        /// [PUT] تعيين مدير أو مسؤول إداري لمسجد معين
        /// </summary>
        Task<GeneralResponse> AssignMosqueAdminAsync(AssignMosqueAdminRequest request);

        /// <summary>
        /// [DELETE] حذف مسجد نهائيًا
        /// </summary>
        Task<GeneralResponse> DeleteMosqueAsync(DeleteMosqueRequest request);

        /// <summary>
        /// [DELETE] إلغاء تعيين مدير من مسجد معين
        /// </summary>
        Task<GeneralResponse> UnassignMosqueAdminAsync(UnassignMosqueAdminRequest request);
    }
}