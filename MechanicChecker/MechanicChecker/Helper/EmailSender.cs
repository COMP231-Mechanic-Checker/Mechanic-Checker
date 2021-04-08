using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace MechanicChecker.Helper
{
    public class EmailSender
    {
        private static string sendgridAPIKey = Startup.Configuration.GetSection("APIKeys")["SendGrid"];
        public static async Task<Response> SendActivationEmail(string activateEmailLink, string toName, string toEmail)
        {
            SendGridClient client = new SendGridClient(sendgridAPIKey);
            EmailAddress from = new EmailAddress("mechanicchecker@outlook.com", "Mechanic Checker");
            EmailAddress to = new EmailAddress(toEmail, toName);

            SendGridMessage msg = MailHelper.CreateSingleTemplateEmail(from, to, "d-e133ee60343547378f2a140ea3841c3e", new { name = toName, link = activateEmailLink });
            Response response = await client.SendEmailAsync(msg);
            return response;
        }

        public static async Task<Response> SendResetPasswordEmail(string resetPasswordLink, string toName, string toEmail)
        {
            var client = new SendGridClient(sendgridAPIKey);
            var from = new EmailAddress("mechanicchecker@outlook.com", "Mechanic Checker");
            var to = new EmailAddress(toEmail, toName);

            var msg = MailHelper.CreateSingleTemplateEmail(from, to, "d-8a2852521a1c47578f046c1ef0ca0cf7", new { name = toName, link = resetPasswordLink });
            var response = await client.SendEmailAsync(msg);
            return response;
        }
    }
}
