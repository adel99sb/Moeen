using Moeen.Api.Shared.Requests.Registration;
using Moeen.Api.Shared.Responses.CircleTeacherAssignment;

namespace Moeen.Api.Core.Contracts.Application
{
    public interface IRegistrationService
    {
        /// <summary>
        /// تسجيل طالب في حلقة دراسية
        /// </summary>
        /// <param name="request">معرف الطالب ومعرف الحلقة</param>
        /// <returns>نتيجة العملية مع رسالة توضيحية</returns>
        Task<OperationResponse> RegisterInCircleAsync(RegisterInCircleRequest request);

        /// <summary>
        /// إلغاء تسجيل طالب من حلقة دراسية
        /// </summary>
        /// <param name="request">معرف الطالب ومعرف الحلقة</param>
        /// <returns>نتيجة العملية مع رسالة توضيحية</returns>
        Task<OperationResponse> UnregisterFromCircleAsync(UnregisterFromCircleRequest request);

        /// <summary>
        /// نقل طالب بين حلقتين دراسيتين
        /// </summary>
        /// <param name="request">معرف الطالب ومعرف الحلقة المصدر ومعرف الحلقة الهدف</param>
        /// <returns>نتيجة العملية مع رسالة توضيحية</returns>
        Task<OperationResponse> TransferStudentAsync(TransferStudentRequest request);
    }
}