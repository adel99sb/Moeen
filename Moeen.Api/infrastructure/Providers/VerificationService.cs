using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moeen.Api.Core.Contracts.infrastructure.Providers;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Moeen.Api.infrastructure.Providers
{
    public class VerificationService : IVerificationService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IEmailService _emailService;
        private readonly ILogger<VerificationService> _logger;
        private const string CachePrefix = "verification_code_";
        private const int MaxCodeLength = 9; // أقصى طول آمن للرمز الرقمي (لتجنب تجاوز int)

        public VerificationService(IMemoryCache memoryCache, IEmailService emailService, ILogger<VerificationService> logger)
        {
            _memoryCache = memoryCache;
            _emailService = emailService;
            _logger = logger;
        }

        public Task<string> GenerateCodeAsync(string key, int length = 6, int expiryMinutes = 10)
        {
            // 1. التحقق من صحة المدخلات
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Key is required.", nameof(key));

            // تحديد الحدود الآمنة
            if (length < 4) length = 4;
            if (length > MaxCodeLength) length = MaxCodeLength; // منع التجاوز
            if (expiryMinutes < 1) expiryMinutes = 1;

            // 2. توليد رمز رقمي آمن
            string code = GenerateNumericCode(length);

            // 3. تخزين الرمز في الكاش
            _memoryCache.Set(
                $"{CachePrefix}{key}",
                code,
                new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(expiryMinutes)
                });

            _logger.LogDebug("تم توليد رمز التحقق للمفتاح {Key} بطول {Length} وينتهي بعد {Expiry} دقيقة", key, length, expiryMinutes);
            return Task.FromResult(code);
        }

        public Task<bool> VerifyCodeAsync(string key, string code)
        {
            // 1. التحقق من المدخلات
            if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(code))
                return Task.FromResult(false);

            string cacheKey = $"{CachePrefix}{key}";
            // 2. البحث عن الرمز المخزن
            if (!_memoryCache.TryGetValue(cacheKey, out string? cachedCode))
            {
                _logger.LogWarning("محاولة تحقق بمفتاح غير موجود أو منتهي الصلاحية: {Key}", key);
                return Task.FromResult(false);
            }

            // 3. المقارنة (حساسة لحالة الأحرف، لكن الأرقام لا تتأثر)
            bool isValid = string.Equals(cachedCode, code, StringComparison.Ordinal);
            if (isValid)
            {
                _memoryCache.Remove(cacheKey);
                _logger.LogInformation("تم التحقق بنجاح للمفتاح {Key}", key);
            }
            else
            {
                _logger.LogWarning("رمز غير صحيح للمفتاح {Key}. المدخل: {Code}", key, code);
            }

            return Task.FromResult(isValid);
        }

        public async Task SendCodeToEmailAsync(string email, string key, string subject = "Verification Code")
        {
            // 1. توليد الرمز
            string code = await GenerateCodeAsync(key); // الطول والصلاحية افتراضيان

            // 2. بناء نص البريد
            string body = $@"
                <html>
                <body>
                    <h3>رمز التحقق الخاص بك</h3>
                    <p>استخدم الرمز التالي لإكمال العملية:</p>
                    <p style='font-size:24px; font-weight:bold;'>{code}</p>
                    <p>هذا الرمز صالح لمدة 10 دقائق.</p>
                </body>
                </html>";

            try
            {
                // 3. إرسال البريد
                await _emailService.SendEmailAsync(email, subject, body, isBodyHtml: true);
                _logger.LogInformation("تم إرسال رمز التحقق إلى {Email} للمفتاح {Key}", email, key);
            }
            catch (Exception ex)
            {
                // 4. إذا فشل الإرسال، نحذف الرمز من الكاش لئلا يبقى صالحاً بلا فائدة
                _memoryCache.Remove($"{CachePrefix}{key}");
                _logger.LogError(ex, "فشل إرسال رمز التحقق إلى {Email} للمفتاح {Key}. تم حذف الرمز.", email, key);
                throw new InvalidOperationException("تعذر إرسال رمز التحقق عبر البريد. حاول مرة أخرى.", ex);
            }
        }

        /// <summary>
        /// توليد رمز رقمي آمن بطول محدد (حد أقصى 9 أرقام)
        /// </summary>
        private static string GenerateNumericCode(int length)
        {
            // طريقة آمنة لتوليد رقم عشوائي دون تجاوز سعة العدد الصحيح
            // باستخدام RandomNumberGenerator لملء مصفوفة بايتات وتحويلها إلى رقم
            if (length <= 0) length = 4;
            if (length > MaxCodeLength) length = MaxCodeLength;

            // أقصى قيمة للرقم: 10^length - 1
            // نحتاج إلى توليد رقم عشوائي بين 0 و (10^length - 1)
            // لكن 10^length قد يكون كبيراً جداً، لذا نستخدم طريقة توليد الأرقام مباشرة كسلسلة

            // البديل الأبسط: توليد سلسلة من الأرقام العشوائية
            byte[] randomBytes = new byte[length];
            RandomNumberGenerator.Fill(randomBytes);

            StringBuilder sb = new StringBuilder(length);
            foreach (byte b in randomBytes)
            {
                // تحويل البايت إلى رقم من 0-9 (باستخدام باقي القسمة على 10)
                sb.Append(b % 10);
            }
            return sb.ToString();
        }
    }
}