using System.Threading.Tasks;

namespace CakeItWebApp.Services.Messaging
{
    public interface ICustomEmilSender
    {
        Task<SendEmailResponse> SendEmailAsync(SendEmailDetails details);
    }
}
