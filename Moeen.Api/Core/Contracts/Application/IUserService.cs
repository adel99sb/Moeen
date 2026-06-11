using Moeen.Shared.Requests.User;
using Moeen.Shared.Responses;

namespace Moeen.Api.Core.Contracts.Application
{
    public interface IUserService
    {
        Task<GeneralResponse> RegisterAsync(RegisterRequest registerRequest);
        Task<GeneralResponse> LoginAsync(LoginRequest loginRequest);
        Task<GeneralResponse> SendVerifyEmailCodeAsync(SendVerifyEmailCodeRequest sendVerifyEmailCodeRequest);
        Task<GeneralResponse> VerifyEmailAsync(VerifyEmailRequest verifyEmailRequest);
        Task<GeneralResponse> ResetPasswordAsync(ResetPasswordRequest resetPasswordRequest);
        Task<GeneralResponse> DeleteUserAsync(Guid userId);
        Task<GeneralResponse> GetAllUsersAsync();
    }
}
