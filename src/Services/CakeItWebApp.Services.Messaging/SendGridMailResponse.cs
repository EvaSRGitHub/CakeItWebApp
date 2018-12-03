using System;
using System.Collections.Generic;
using System.Text;

namespace CakeItWebApp.Services.Messaging
{
    //collection of all errors received from the mail sending via send grid
    public class SendGridMailResponse
    {
        public ICollection<SendGridResponseErrors> Errors { get; set; }
    }
}
