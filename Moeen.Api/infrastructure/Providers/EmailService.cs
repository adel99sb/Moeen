using Moeen.Api.Application;
using Moeen.Api.Core.Contracts.infrastructure.Providers;
using Moeen.Api.infrastructure.Configurations;
using System.Net;
using System.Net.Mail;

namespace Moeen.Api.infrastructure.Providers
{
    public class EmailService : IEmailService
    {
        private readonly SmtpSettings _smtpSettings;

        public EmailService()
        {
            _smtpSettings = AppSettings.Instance.SmtpSettings;
        }

        public async Task<bool> SendEmailAsync(string toEmail, string subject, string code, string lang, bool isRest = false)
        {
            var smtpClient = CreateSmtpClient();
            string htmlContent = emailContent(lang, isRest, code);
            var mailMessage = CreateMailMessage(toEmail, subject, htmlContent);
            try
            {
                await smtpClient.SendMailAsync(mailMessage);
                return true;
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
                return false;
            }
        }

        private SmtpClient CreateSmtpClient()
        {
            return new SmtpClient
            {
                Host = _smtpSettings.Host,
                Port = _smtpSettings.Port,
                EnableSsl = _smtpSettings.EnableSsl,
                Credentials = new NetworkCredential(_smtpSettings.SenderEmail, _smtpSettings.SenderPassword)
            };
        }
        private string emailContent(string lang, bool isRest, string code)
        {
            string mainTemplatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Shared", "ValueObject");
            string usedTemplatePath;
            string textToReplace = "{{verificationCode}}";
            switch (lang)
            {
                case "ar":
                    if (isRest)
                    {
                        usedTemplatePath = Path.Combine(mainTemplatePath, "reset-password-ar.html");
                        textToReplace = "{{resetLink}}";
                    }
                    else
                        usedTemplatePath = Path.Combine(mainTemplatePath, "email-verification-ar.html");
                    break;
                default:
                    if (isRest)
                    {
                        usedTemplatePath = Path.Combine(mainTemplatePath, "reset-password-en.html");
                        textToReplace = "{{resetLink}}";
                    }
                    else
                        usedTemplatePath = Path.Combine(mainTemplatePath, "email-verification-en.html");
                    break;
            }
            string htmlContent = File.ReadAllText(usedTemplatePath);
            htmlContent = htmlContent.Replace(textToReplace, code);
            return htmlContent;
        }
        private MailMessage CreateMailMessage(string toEmail, string subject, string body)
        {
            return new MailMessage
            {
                From = new MailAddress(_smtpSettings.SenderEmail, "Moeen Support"),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            }.AddRecipient(toEmail);
        }
    }
}