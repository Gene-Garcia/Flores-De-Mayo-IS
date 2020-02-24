using Microsoft.AspNet.Identity;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Flores_De_Mayo_Information_System.Actions
{
    public class Mailer
    {

        private static string senderEmail = "FloresDeMayo@Sinisian.com";
        private static string sendUsername = "Alayan President";

        public static async Task SendEmail(IdentityMessage message)
        {
            string apiKey = "";
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(senderEmail, sendUsername);

            var subject = message.Subject;
            var to = new EmailAddress(message.Destination);

            var msg = MailHelper.CreateSingleEmail(from, to, subject, "", message.Body);

            if (client != null)
            {
                await client.SendEmailAsync(msg);
            }
            else
            {
                Trace.TraceError("Failed to create Web transport.");
                await Task.FromResult(0);
            }

        }
    }
}
