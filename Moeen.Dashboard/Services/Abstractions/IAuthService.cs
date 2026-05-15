using Moeen.Shared.Requests.Enrollment;
using Moeen.Shared.Requests.Identity;
using Moeen.Shared.Responses;
using Moeen.Shared.Responses.Identity;

namespace Moeen.Dashboard.Services.Abstractions
{
    public interface IAuthService
    {
        Task<AuthResponse> Login(LoginRequest request);
        Task<GeneralResponse> Register(RegisterRequest request);
        Task<GeneralResponse> Search(SearchMembersRequest request);
        Task<GeneralResponse> SendVerifyEmailCode(SendVerifyEmailCodeRequest request);
        Task<GeneralResponse> VerifyEmail(VerifyEmailRequest request);

    }
}
