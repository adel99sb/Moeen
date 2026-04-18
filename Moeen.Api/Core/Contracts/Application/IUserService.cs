using Moeen.Api.Shared.Requests;
using Moeen.Api.Shared.Requests.Identity;
using Moeen.Api.Shared.Responses;

namespace Moeen.Api.Core.Contracts.Application
{
    public interface IUserService
    {
        Task<GeneralResponse> RegisterAsync(RegisterRequest registerRequest);
        Task<GeneralResponse> LoginAsync(LoginRequest loginRequest);
        Task<GeneralResponse> GetAllUsersAsync(PaginationRequest paginationRequest, string? keyword);
        Task<GeneralResponse> ChangeUserEmailAsync(string email);
        Task<GeneralResponse> GetUserByIdAsync(Guid userId);
        Task<GeneralResponse> SendVerifyEmailCodeAsync(SendVerifyEmailCodeRequest sendVerifyEmailCodeRequest);
        Task<GeneralResponse> VerifyEmailAsync(VerifyEmailRequest verifyEmailRequest);
        Task<GeneralResponse> SendPasswordResetUrlAsync(SendPasswordResetUrlRequest sendPasswordResetUrlRequest);
        Task<GeneralResponse> ResetPasswordAsync(ChangePasswordRequest changePasswordRequest);
    }
}
