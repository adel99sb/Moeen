using Moeen.Shared.Requests.CircleTeacherAssignment;
using Moeen.Shared.Requests.HalqaTeacherAssignment;
using Moeen.Shared.Responses;
using Moeen.Shared.Responses.CircleTeacherAssignment;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Moeen.Api.Core.Contracts.Application
{
    public interface IHalqaTeacherAssignmentService
    {
        /// <summary>
        /// تعيين معلم مسؤول عن حلقة دراسية
        /// </summary>
        Task<GeneralResponse> AssignTeacherToHalqaAsync(AssignTeacherToHalqaRequest request);

        /// <summary>
        /// إزالة معلم من الإشراف على حلقة دراسية
        /// </summary>
        Task<GeneralResponse> RemoveTeacherFromHalqaAsync(RemoveTeacherFromHalqaRequest request);

        /// <summary>
        /// [GET] جلب الحلقات المسندة لمعلم محدد (النشطة أو مع التاريخ)
        /// </summary>
        Task<GeneralResponse> GetHalqasByTeacherAsync(GetHalqasByTeacherRequest request);

        /// <summary>
        /// [GET] جلب المعلمين المسندين لحلقة محددة
        /// </summary>
        Task<GeneralResponse> GetTeachersByHalqaAsync(GetTeachersByHalqaRequest request);

        /// <summary>
        /// [PUT] استبدال معلم بآخر في نفس الحلقة
        /// </summary>
        Task<GeneralResponse> ReplaceTeacherInHalqaAsync(ReplaceTeacherRequest request);
    }
}