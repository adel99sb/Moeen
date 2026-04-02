using Moeen.Api.Shared.Requests.CircleTeacherAssignment;
using Moeen.Api.Shared.Responses.CircleTeacherAssignment;
using System.Threading.Tasks;

namespace Moeen.Api.Core.Contracts.Application
{
    public interface ICircleTeacherAssignmentService
    {
        /// <summary>
        /// تعيين معلم مسؤول عن حلقة دراسية
        /// </summary>
        /// <param name="request">معرف الحلقة، معرف المعلم، وتحديد ما إذا كان المعلم أساسياً</param>
        /// <returns>نتيجة العملية مع رسالة توضيحية</returns>
        Task<OperationResponse> AssignTeacherToCircleAsync(AssignTeacherToCircleRequest request);

        /// <summary>
        /// إزالة معلم من الإشراف على حلقة دراسية
        /// </summary>
        /// <param name="request">معرف الحلقة ومعرف المعلم</param>
        /// <returns>نتيجة العملية مع رسالة توضيحية</returns>
        Task<OperationResponse> RemoveTeacherFromCircleAsync(RemoveTeacherFromCircleRequest request);
    }
}