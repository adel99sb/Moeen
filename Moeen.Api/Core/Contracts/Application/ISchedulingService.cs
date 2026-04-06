using Moeen.Api.Shared.Requests.Scheduling;
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
    }
}