using Moeen.Shared.Requests.Halqa;
using Moeen.Shared.Responses.Halqa;
using System.Threading.Tasks;

namespace Moeen.Api.Core.Contracts.Application
{
    public interface IHalqaCommandService
    {
        /// <summary>
        /// إنشاء حلقة جديدة
        /// </summary>
        /// <param name="request">بيانات الحلقة الجديدة</param>
        /// <returns>بيانات الحلقة المنشأة</returns>
        Task<HalqaDto> CreateHalqaAsync(CreateHalqaRequest request);

        /// <summary>
        /// تحديث بيانات حلقة موجودة
        /// </summary>
        /// <param name="request">معرف الحلقة والبيانات المراد تحديثها</param>
        /// <returns>بيانات الحلقة بعد التحديث</returns>
        Task<HalqaDto> UpdateHalqaAsync(UpdateHalqaRequest request);

        /// <summary>
        /// حذف حلقة
        /// </summary>
        /// <param name="request">معرف الحلقة المراد حذفها</param>
        /// <returns>true إذا تم الحذف بنجاح</returns>
        Task<bool> DeleteHalqaAsync(DeleteHalqaRequest request);

        /// <summary>
        /// إعادة تعيين معلم للحلقة
        /// </summary>
        /// <param name="request">معرف الحلقة والمعلم الجديد</param>
        /// <returns>بيانات الحلقة بعد إعادة تعيين المعلم</returns>
        Task<HalqaDto> ReassignTeacherAsync(ReassignHalqaTeacherRequest request);

        /// <summary>
        /// نقل الحلقة إلى فوج آخر
        /// </summary>
        /// <param name="request">معرف الحلقة والفوج الجديد</param>
        /// <returns>بيانات الحلقة بعد النقل</returns>
        Task<HalqaDto> MoveToFoujAsync(MoveHalqaToFoujRequest request);
    }
}