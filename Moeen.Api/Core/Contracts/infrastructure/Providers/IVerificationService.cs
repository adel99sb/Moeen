using System.Threading.Tasks;

namespace Moeen.Api.Core.Contracts.infrastructure.Providers
{
    public interface IVerificationService
    {
        Task<string> GenerateCodeAsync(string key, int length = 6, int expiryMinutes = 10);
        Task<bool> VerifyCodeAsync(string key, string code);
        Task SendCodeToEmailAsync(string email, string key, string subject = "Verification Code");
    }
}
