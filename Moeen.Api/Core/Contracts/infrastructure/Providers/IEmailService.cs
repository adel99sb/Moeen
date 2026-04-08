using System.Threading.Tasks;

namespace Moeen.Api.Core.Contracts.infrastructure.Providers
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body, bool isBodyHtml = true);
    }
}
