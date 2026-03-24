using Moeen.Api.Shared.Requests.Points;
using Moeen.Api.Shared.Responses.Points;
using System.Threading.Tasks;

namespace Moeen.Api.Core.Contracts.Application
{
    public interface IPointsService
    {
        /// <summary>
        /// إعداد نظام النقاط بتعيين عدد النقاط لكل تقدير
        /// </summary>
        /// <param name="request">قائمة تعيينات الدرجة -> نقاط</param>
        /// <returns>حالة النجاح</returns>
        Task<SetupPointsSystemResponse> SetupPointsSystemAsync(SetupPointsSystemRequest request);

        /// <summary>
        /// الحصول على رصيد نقاط الطالب الحالي
        /// </summary>
        /// <param name="request">معرف الطالب</param>
        /// <returns>رصيد النقاط</returns>
        Task<GetStudentPointsResponse> GetStudentPointsAsync(GetStudentPointsRequest request);

        /// <summary>
        /// الحصول على سجل النقاط المفصل للطالب
        /// </summary>
        /// <param name="request">معرف الطالب</param>
        /// <returns>قائمة المعاملات مع العدد الكلي</returns>
        Task<GetPointsHistoryResponse> GetPointsHistoryAsync(GetPointsHistoryRequest request);
    }
}