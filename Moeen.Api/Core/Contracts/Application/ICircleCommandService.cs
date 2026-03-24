using Moeen.Api.Shared.Requests.Circle;
using Moeen.Api.Shared.Responses.Circle;
using System.Threading.Tasks;

namespace Moeen.Api.Core.Contracts.Application
{
    public interface ICircleCommandService
    {
        /// <summary>
        /// إنشاء حلقة جديدة
        /// </summary>
        /// <param name="request">بيانات الحلقة الجديدة</param>
        /// <returns>بيانات الحلقة المنشأة</returns>
        Task<CircleDto> CreateCircleAsync(CreateCircleRequest request);

        /// <summary>
        /// تحديث بيانات حلقة موجودة
        /// </summary>
        /// <param name="request">معرف الحلقة والبيانات المراد تحديثها</param>
        /// <returns>بيانات الحلقة بعد التحديث</returns>
        Task<CircleDto> UpdateCircleAsync(UpdateCircleRequest request);

        /// <summary>
        /// حذف حلقة
        /// </summary>
        /// <param name="request">معرف الحلقة المراد حذفها</param>
        /// <returns>true إذا تم الحذف بنجاح</returns>
        Task<bool> DeleteCircleAsync(DeleteCircleRequest request);
        // أو إذا أردت استخدام DeleteCircleResponse:
        // Task<DeleteCircleResponse> DeleteCircleAsync(DeleteCircleRequest request);
    }
}