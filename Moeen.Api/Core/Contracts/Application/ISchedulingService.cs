using Moeen.Api.Shared.Requests.Scheduling;
using Moeen.Api.Shared.Responses;
using Moeen.Api.Shared.Responses.Scheduling;
using System.Threading.Tasks;

namespace Moeen.Api.Core.Contracts.Application
{
    public interface ISchedulingService
    {
        /// <summary>
        /// إنشاء جدول لحلقة
        /// </summary>
        Task<ScheduleResponseDto> CreateCircleScheduleAsync(CreateCircleScheduleRequest request);

        /// <summary>
        /// تخصيص جدول لمعلم
        /// </summary>
        Task<AssignScheduleToTeacherResponse> AssignScheduleToTeacherAsync(AssignScheduleToTeacherRequest request);

        /// <summary>
        /// [GET] جلب تفاصيل جدول حلقة معينة بمعرفها
        /// </summary>
        Task<ScheduleResponseDto> GetCircleScheduleByIdAsync(GetCircleScheduleByIdRequest request);

        /// <summary>
        /// [GET] جلب جميع الجداول مع التصفح والتصفية
        /// </summary>
        Task<PagedList<ScheduleResponseDto>> GetAllSchedulesAsync(GetAllSchedulesRequest request);

        /// <summary>
        /// [PUT] تحديث تفاصيل جدول حلقة
        /// </summary>
        Task<ScheduleResponseDto> UpdateCircleScheduleAsync(UpdateCircleScheduleRequest request);

        /// <summary>
        /// [DELETE] حذف جدول نهائيًا
        /// </summary>
        Task<ScheduleOperationResponse> DeleteScheduleAsync(DeleteScheduleRequest request);
    }
}