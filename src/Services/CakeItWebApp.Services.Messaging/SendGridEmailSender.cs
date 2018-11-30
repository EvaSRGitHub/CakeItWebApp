using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace CakeItWebApp.Services.Messaging
{
    public class SendGridEmailSender : IEmailSender
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
           
            var msg = MailHelper.CreateSingleEmail(from, to, subject, content, null);

            var response = await client.SendEmailAsync(msg);

            return new SendEmailResponse();
        }
    }
}
