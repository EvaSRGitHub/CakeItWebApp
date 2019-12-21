namespace CakeItWebApp.Services.Messaging
{
    //parsed properties of json response from the send e-mail via send grid
    public class SendGridResponseErrors
    {
        public string Message { get; set; }

        public string Field { get; set; }

        public string HelpLink { get; set; }
    }
}