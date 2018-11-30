namespace CakeItWebApp.Services.Messaging
{
    public class SendEmailResponse
    {
        public bool Successful => ErrorMessage == null;

        public string ErrorMessage { get; set; }
    }
}