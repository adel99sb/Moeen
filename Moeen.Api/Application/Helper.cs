using Microsoft.AspNetCore.Mvc;
using Moeen.Shared.Responses;
using System.Net.Mail;

namespace Moeen.Api.Application
{
    public static class Helper
    {
        public static MailMessage AddRecipient(this MailMessage mailMessage, string toEmail)
        {
            mailMessage.To.Add(toEmail);
            return mailMessage;
        }
        public static IActionResult ToActionResult(this GeneralResponse response)
        {
            return new ObjectResult(response)
            {
                StatusCode = response.StatusCode
            };
        }
    }
}
