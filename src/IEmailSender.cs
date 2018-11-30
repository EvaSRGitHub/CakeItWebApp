using System;
using System.Threading.Tasks;

public interface IEmailSender
{
    Task<SendEmailResponse> SendEmailAsync(SendEmailDetails details);
}
