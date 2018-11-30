using System.Threading.Tasks;

namespace CakeItWebApp.Services.Messaging
{
    public interface IEmailSender
    {
        Task<SendEmailResponse> SendEmailAsync(SendEmailDetails details);
    }
}
