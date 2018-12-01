using System.Collections.Generic;

namespace CakeItWebApp.Services.Messaging
{
    public class SendEmailResponse
    {
        public bool Successful => !(Errors?.Count > 0);

        public List<string> Errors{ get; set; }
    }
}