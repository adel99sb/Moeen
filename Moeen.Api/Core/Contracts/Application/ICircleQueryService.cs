using Moeen.Api.Shared.Requests.CircleQuery;
using Moeen.Api.Shared.Responses.Circle;
using Moeen.Api.Shared.Responses.CircleQuery;
using System.Threading.Tasks;

namespace Moeen.Api.Core.Contracts.Application
{
    public interface ICircleQueryService
    {
        /// <summary>
        /// الحصول على تفاصيل حلقة محددة
        /// </summary>
        /// <param name="request">معرف الحلقة</param>
        /// <returns>بيانات الحلقة</returns>
        Task<CircleDto> GetCircleByIdAsync(GetCircleByIdRequest request);

        /// <summary>
        /// الحصول على قائمة الطلاب المسجلين في الحلقة مع إمكانية التصفية والتصفح
        /// </summary>
        /// <param name="request">معرف الحلقة ومعايير التصفية</param>
        /// <returns>قائمة الطلاب مع معلومات التصفح</returns>
        Task<CircleStudentsResponse> GetCircleStudentsAsync(GetCircleStudentsRequest request);

        /// <summary>
        /// الحصول على عدد الطلاب المسجلين في الحلقة
        /// </summary>
        /// <param name="request">معرف الحلقة</param>
        /// <returns>عدد الطلاب</returns>
        Task<CircleStudentsCountResponse> GetCircleStudentsCountAsync(GetCircleStudentsCountRequest request);
    }
}