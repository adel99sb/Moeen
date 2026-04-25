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

        /// <summary>
        /// إعادة تعيين معلم للحلقة
        /// </summary>
        /// <param name="request">معرف الحلقة والمعلم الجديد</param>
        /// <returns>بيانات الحلقة بعد إعادة تعيين المعلم</returns>
        Task<CircleDto> ReassignTeacherAsync(ReassignCircleTeacherRequest request);

        /// <summary>
        /// نقل الحلقة إلى فوج آخر
        /// </summary>
        /// <param name="request">معرف الحلقة والفوج الجديد</param>
        /// <returns>بيانات الحلقة بعد النقل</returns>
        Task<CircleDto> MoveToFoujAsync(MoveCircleToFoujRequest request);
    }
}