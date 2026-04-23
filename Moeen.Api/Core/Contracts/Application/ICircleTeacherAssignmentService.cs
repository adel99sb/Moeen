using Moeen.Api.Shared.Requests.CircleTeacherAssignment;
using Moeen.Api.Shared.Responses.CircleTeacherAssignment;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Moeen.Api.Core.Contracts.Application
{
    public interface ICircleTeacherAssignmentService
    {
        /// <summary>
        /// تعيين معلم مسؤول عن حلقة دراسية
        /// </summary>
        Task<OperationResponseDto> AssignTeacherToCircleAsync(AssignTeacherToCircleRequest request);

        /// <summary>
        /// إزالة معلم من الإشراف على حلقة دراسية
        /// </summary>
        Task<OperationResponseDto> RemoveTeacherFromCircleAsync(RemoveTeacherFromCircleRequest request);

        /// <summary>
        /// [GET] جلب الحلقات المسندة لمعلم محدد (النشطة أو مع التاريخ)
        /// </summary>
        Task<List<CircleAssignmentDto>> GetCirclesByTeacherAsync(GetCirclesByTeacherRequest request);

        /// <summary>
        /// [GET] جلب المعلمين المسندين لحلقة محددة
        /// </summary>
        Task<List<CircleAssignmentDto>> GetTeachersByCircleAsync(GetTeachersByCircleRequest request);

        /// <summary>
        /// [PUT] استبدال معلم بآخر في نفس الحلقة
        /// </summary>
        Task<OperationResponseDto> ReplaceTeacherInCircleAsync(ReplaceTeacherRequest request);
    }
}