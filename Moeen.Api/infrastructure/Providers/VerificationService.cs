using Microsoft.Extensions.Caching.Memory;
using Moeen.Api.Core.Contracts.infrastructure.Providers;
using System.Net;

namespace Moeen.Api.infrastructure.Providers
{
    public class VerificationService : IVerificationService
    {
        private readonly IEmailService _emailService;
        private readonly IMemoryCache _cache;


        public VerificationService(IEmailService emailService, IMemoryCache cache)
        {
            _emailService = emailService;
            _cache = cache;
        }

        public async Task SendVerificationCodeAsync(Guid userId, string email, string subject, string lang)
        {
            string verificationCode = GenerateVerificationCode();

            var result = await _emailService.SendEmailAsync(email, subject, verificationCode, lang);
            if (result)
            {
                _cache.Set($"VerificationCode_{userId}", verificationCode, TimeSpan.FromMinutes(10));
            }
            else
            {
                throw new Exception("Send verification code faild");
            }
        }

        public async Task SendRestpageUrlAsync(string email, string subject, string token, string pageUrl, string lang)
        {
            var encodedToken = WebUtility.UrlEncode(token);
            var url = $"{pageUrl}?email={email}&&resetToken={encodedToken}";

            var result = await _emailService.SendEmailAsync(email, subject, url, lang, true);
            if (!result)
                throw new Exception("Send verification code faild");
        }

        public async Task<bool> VerifyCodeAsync(Guid userId, string code)
        {
            string storedCode = _cache.Get<string>($"VerificationCode_{userId}");

            if (storedCode != null && storedCode == code)
            {
                _cache.Remove($"VerificationCode_{userId}");
                return true;
            }

            return false;
        }

        private string GenerateVerificationCode()
        {
            Random random = new Random();
            return random.Next(100000, 999999).ToString();
        }
    }
}