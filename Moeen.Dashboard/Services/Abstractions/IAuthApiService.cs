using Moeen.Shared.Requests.User;
using Moeen.Shared.Responses.User;

namespace Moeen.Dashboard.Services.Abstractions
{
    public interface IAuthApiService
    {
        Task<(bool Success, string Message, LoginResponse? Data)> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
        Task<(bool Success, string Message)> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
        Task<(bool Success, string Message)> VerifyEmailAsync(VerifyEmailRequest request, CancellationToken cancellationToken = default);
        Task<(bool Success, string Message)> ResetPasswordAsync(ResetPasswordRequest request, CancellationToken cancellationToken = default);
        Task<(bool Success, string Message)> SendVerifyCodeAsync(SendVerifyEmailCodeRequest request, CancellationToken cancellationToken = default);
    }
}
