using Moeen.Shared.Requests.Registration;
using Moeen.Shared.Responses;
using System.Threading.Tasks;

namespace Moeen.Api.Core.Contracts.Application
{
    public interface IRegistrationService
    {   
        /// <summary>
        /// تسجيل طالب في حلقة دراسية
        /// </summary>
        Task<GeneralResponse> RegisterInCircleAsync(RegisterInCircleRequest request);

        /// <summary>
        /// إلغاء تسجيل طالب من حلقة دراسية
        /// </summary>
        Task<GeneralResponse> UnregisterFromCircleAsync(UnregisterFromCircleRequest request);

        /// <summary>
        /// نقل طالب بين حلقتين دراسيتين
        /// </summary>
        Task<GeneralResponse> TransferStudentAsync(TransferStudentRequest request);

        /// <summary>
        /// [GET] جلب جميع الطلاب المسجلين في حلقة مع التصفح والتصفية
        /// </summary>
        Task<GeneralResponse> GetCircleStudentsAsync(GetCircleRegisteredStudentsRequest request);
    }
}