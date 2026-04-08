using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moeen.Api.Core.Contracts.infrastructure.Providers;
using Moeen.Api.infrastructure.Configurations;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;

namespace Moeen.Api.infrastructure.Providers
{
    public class EmailService : IEmailService
    {
        private readonly SmtpSettings _smtpSettings;
        private readonly ILogger<EmailService> _logger;
        private const int MaxBodySize = 10 * 1024 * 1024; // 10 MB حد أقصى لحجم نص البريد

        public EmailService(IOptions<SmtpSettings> smtpSettings, ILogger<EmailService> logger)
        {
            _smtpSettings = smtpSettings.Value;
            _logger = logger;
        }

        /// <summary>
        /// إرسال بريد إلكتروني إلى مستلم واحد
        /// </summary>
        public async Task SendEmailAsync(string toEmail, string subject, string body, bool isBodyHtml = true)
        {
            // 1. التحقق من صحة المستلم
            if (string.IsNullOrWhiteSpace(toEmail))
                throw new ArgumentException("بريد المستلم مطلوب.", nameof(toEmail));

            // 2. التحقق من صيغة البريد الإلكتروني
            if (!IsValidEmail(toEmail))
                throw new ArgumentException("بريد المستلم غير صحيح.", nameof(toEmail));

            // 3. التحقق من وجود نص البريد
            if (string.IsNullOrWhiteSpace(body))
                throw new ArgumentException("محتوى البريد مطلوب.", nameof(body));

            // 4. التحقق من حجم النص
            if (body.Length > MaxBodySize)
                throw new ArgumentException($"حجم نص البريد يتجاوز الحد المسموح ({MaxBodySize / (1024 * 1024)} MB).");

            // 5. التأكد من إعدادات SMTP الأساسية
            if (string.IsNullOrWhiteSpace(_smtpSettings.Host))
                throw new InvalidOperationException("SMTP Host غير مضبوط.");
            if (string.IsNullOrWhiteSpace(_smtpSettings.FromEmail))
                throw new InvalidOperationException("البريد المرسل (FromEmail) غير مضبوط.");
            if (string.IsNullOrWhiteSpace(_smtpSettings.Username) || string.IsNullOrWhiteSpace(_smtpSettings.Password))
                throw new InvalidOperationException("اسم المستخدم أو كلمة المرور لـ SMTP غير مضبوطة.");

            // 6. بناء كائن الرسالة
            using var message = new MailMessage
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = isBodyHtml,
                From = string.IsNullOrWhiteSpace(_smtpSettings.FromName)
                    ? new MailAddress(_smtpSettings.FromEmail)
                    : new MailAddress(_smtpSettings.FromEmail, _smtpSettings.FromName)
            };
            message.To.Add(new MailAddress(toEmail));

            // 7. تكوين عميل SMTP
            using var smtpClient = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port)
            {
                EnableSsl = _smtpSettings.EnableSsl,
                Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password),
                Timeout = 30000 // 30 ثانية مهلة الإرسال
            };

            try
            {
                // 8. الإرسال (يمكن إضافة CancellationToken لاحقاً إذا عدلت الواجهة)
                await smtpClient.SendMailAsync(message);
                _logger.LogInformation("تم إرسال البريد إلى {ToEmail} بموضوع {Subject}", toEmail, subject);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "فشل إرسال البريد إلى {ToEmail}", toEmail);
                throw new InvalidOperationException("فشل إرسال البريد. راجع السجلات.", ex);
            }
        }

        // دالة مساعدة للتحقق من صيغة البريد
        private static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}