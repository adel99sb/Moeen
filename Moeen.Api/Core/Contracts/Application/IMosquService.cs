using Moeen.Shared.Requests.Mosuq;
using Moeen.Shared.Responses.Circle;
using Moeen.Shared.Responses.CircleTeacherAssignment;
using Moeen.Shared.Responses.Enrollment;
using Moeen.Shared.Responses.Mosuq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Moeen.Api.Core.Contracts.Application
{
    public interface IMosquService
    {
        Task<bool> AddMosqu(AddMosquReq req);
        Task<GetAllMosqusResponse> GetAllMosqus();

        /// <summary>
        /// [GET] جلب تفاصيل مسجد محدد بمعرفه مع المعلومات الكاملة
        /// </summary>
        Task<MosqueDto> GetMosqueByIdAsync(GetMosqueByIdRequest request);

        /// <summary>
        /// [GET] جلب الحلقات الدراسية التابعة لمسجد معين
        /// </summary>
        Task<List<CircleDto>> GetCirclesByMosqueAsync(GetCirclesByMosqueRequest request);

        /// <summary>
        /// [GET] جلب المعلمين المعينين في مسجد معين
        /// </summary>
        Task<List<TeacherDto>> GetTeachersByMosqueAsync(GetTeachersByMosqueRequest request);

        /// <summary>
        /// [GET] الحصول على إحصائيات مسجد
        /// </summary>
        Task<MosqueStatisticsDto> GetMosqueStatisticsAsync(GetMosqueStatisticsRequest request);

        /// <summary>
        /// [GET] جلب المساجد القريبة من موقع المستخدم الحالي
        /// </summary>
        Task<List<MosqueDto>> GetNearbyMosquesAsync(GetNearbyMosquesRequest request);

        /// <summary>
        /// [PUT] تحديث معلومات مسجد أساسي
        /// </summary>
        Task<MosqueDto> UpdateMosqueInfoAsync(UpdateMosqueInfoRequest request);

        /// <summary>
        /// [PUT] تعيين مدير أو مسؤول إداري لمسجد معين
        /// </summary>
        Task<OperationResponseDto> AssignMosqueAdminAsync(AssignMosqueAdminRequest request);

        /// <summary>
        /// [DELETE] حذف مسجد نهائيًا
        /// </summary>
        Task<OperationResponseDto> DeleteMosqueAsync(DeleteMosqueRequest request);

        /// <summary>
        /// [DELETE] إلغاء تعيين مدير من مسجد معين
        /// </summary>
        Task<OperationResponseDto> UnassignMosqueAdminAsync(UnassignMosqueAdminRequest request);
    }
}
