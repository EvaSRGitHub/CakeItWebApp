using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CakeItWebApp.Services.Messaging
{
    public class SendGridEmailSender : ICustomEmilSender
    {
        private readonly IServiceProvider provider;

        public SendGridEmailSender(IServiceProvider provider)
        {
            this.provider = provider;
        }

        public async Task<SendEmailResponse> SendEmailAsync(SendEmailDetails details)
        {
            var apiKey = provider.GetService<IConfiguration>()["SendGridKey"];
               
            var client = new SendGridClient(apiKey);

            var from = new EmailAddress(details.FromEmail, details.FromName);

            var subject = details.Subject;

            var to = new EmailAddress(details.ToEmail, details.ToName);

            var content = details.Content;
           
            var msg = MailHelper.CreateSingleEmail(from, to, subject, null, content);

            var response = await client.SendEmailAsync(msg);
            
            if (response.StatusCode != System.Net.HttpStatusCode.Accepted)
            {
                var bodyResult = await response.Body.ReadAsStringAsync();
                var deserializeResponse = JsonConvert.DeserializeObject<SendGridMailResponse>(bodyResult);
                var errors = new SendEmailResponse { Errors = deserializeResponse?.Errors.Select(e => e.Message).ToList() };

                return errors;
            }

            return new SendEmailResponse();
        }
    }
}
