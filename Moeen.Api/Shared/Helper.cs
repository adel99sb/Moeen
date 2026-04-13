using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;

namespace Moeen.Api.Shared
{
    public static class Helper
    {
        public static MailMessage AddRecipient(this MailMessage mailMessage, string toEmail)
        {
            mailMessage.To.Add(toEmail);
            return mailMessage;
        }
        //public static IActionResult ToActionResult(this GeneralResponse response)
        //{
        //    return new ObjectResult(response)
        //    {
        //        StatusCode = response.StatusCode
        //    };
        //}
    }
}
