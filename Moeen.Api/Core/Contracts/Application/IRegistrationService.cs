using Moeen.Shared.Requests.Registration;
using Moeen.Shared.Responses;
using Moeen.Shared.Responses.CircleTeacherAssignment;
using Moeen.Shared.Responses.Registration;
using System.Threading.Tasks;

namespace Moeen.Api.Core.Contracts.Application
{
    public interface IRegistrationService
    {   
        /// <summary>
        /// تسجيل طالب في حلقة دراسية
        /// </summary>
        Task<OperationResponseDto> RegisterInCircleAsync(RegisterInCircleRequest request);

        /// <summary>
        /// إلغاء تسجيل طالب من حلقة دراسية
        /// </summary>
        Task<OperationResponseDto> UnregisterFromCircleAsync(UnregisterFromCircleRequest request);

        /// <summary>
        /// نقل طالب بين حلقتين دراسيتين
        /// </summary>
        Task<OperationResponseDto> TransferStudentAsync(TransferStudentRequest request);

        /// <summary>
        /// [GET] جلب تفاصيل تسجيل طالب في حلقة بمعرف التسجيل
        /// </summary>
        Task<RegistrationDto> GetRegistrationByIdAsync(GetRegistrationByIdRequest request);

        /// <summary>
        /// [GET] جلب جميع الطلاب المسجلين في حلقة مع التصفح والتصفية
        /// </summary>
        Task<PagedList<CircleStudentDto>> GetCircleStudentsAsync(GetCircleRegisteredStudentsRequest request);

        /// <summary>
        /// [PUT] تحديث حالة التسجيل
        /// </summary>
        Task<RegistrationDto> UpdateRegistrationStatusAsync(UpdateRegistrationStatusRequest request);
    }
}