using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SendGrid;
using SendGrid.Helpers.Mail;
using MechanicChecker.Models;

namespace MechanicChecker.Helper
{
    public class EmailSender
    {
        public static async Task<Response> SendActivationEmail(ExternalAPIsContext contextAPIs, string activateEmailLink, string toName, string toEmail)
        {
            string sendgridAPIKey = GetSendGridAPIKey(contextAPIs);

            SendGridClient client = new SendGridClient(sendgridAPIKey);
            EmailAddress from = new EmailAddress("mechanicchecker@outlook.com", "Mechanic Checker");
            EmailAddress to = new EmailAddress(toEmail, toName);

            SendGridMessage msg = MailHelper.CreateSingleTemplateEmail(from, to, "d-e133ee60343547378f2a140ea3841c3e", new { name = toName, link = activateEmailLink });
            Response response = await client.SendEmailAsync(msg);

            return response;
        }

        public static async Task<Response> SendResetPasswordEmail(ExternalAPIsContext contextAPIs, string resetPasswordLink, string toName, string toEmail)
        {
            string sendgridAPIKey = GetSendGridAPIKey(contextAPIs);

            SendGridClient client = new SendGridClient(sendgridAPIKey);
            EmailAddress from = new EmailAddress("mechanicchecker@outlook.com", "Mechanic Checker");
            EmailAddress to = new EmailAddress(toEmail, toName);

            SendGridMessage msg = MailHelper.CreateSingleTemplateEmail(from, to, "d-8a2852521a1c47578f046c1ef0ca0cf7", new { name = toName, link = resetPasswordLink });
            Response response = await client.SendEmailAsync(msg);
  
            return response;
        }

        private static string GetSendGridAPIKey(ExternalAPIsContext contextAPIs)
        {
            string apiKeyOwner = Startup.Configuration.GetSection("APIKeyOwner")["SendGrid"];
            ExternalAPIs sendGridAPI = contextAPIs.GetApiByService("DeveloperAPI SendGrid", apiKeyOwner);

            // we assume that the key is active and sendgrid api has 100 max quota hard limit with daily reset
            contextAPIs.activateAPI("DeveloperAPI SendGrid", apiKeyOwner); // reduce quota by 1
            
            return sendGridAPI.APIKey;
        }
    }
}
