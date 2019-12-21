using CakeItWebApp.Services.Messaging;
using CakeItWebApp.ViewModels.Cart;
using CakeItWebApp.ViewModels.Orders;
using CakeWebApp.Services.Common.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CakeWebApp.Services.Common.CommonServices
{
    public class EmailService : IEmailService
    {
        private readonly IServiceProvider provider;
        private readonly ILogger<EmailService> logger;
        private readonly ICartService cartService;

        public EmailService(IServiceProvider provider, ILogger<EmailService> logger, ICartService cartService)
        {
            this.provider = provider;
            this.logger = logger;
            this.cartService = cartService;
        }

        public async Task SendOrderMail(OrderDetailsViewModel model)
        {
            string content = CreateEmaiContent(model.Email);

            SendEmailDetails details = new SendEmailDetails()
            {
                FromEmail = this.provider.GetService<IConfiguration>()["VerificationEmailDetails:FromEmail"],
                FromName = this.provider.GetService<IConfiguration>()["VerificationEmailDetails:FromName"],
                ToEmail = model.Email,
                ToName = model.FirstName + " " + model.LastName,
                Subject = "Confirmation of you CakeIt order.",
                Content = content
            };

            var sendEmailResult = await this.provider.GetService<ICustomEmilSender>().SendEmailAsync(details);

            if (!sendEmailResult.Successful)
            {
                foreach (var error in sendEmailResult.Errors)
                {
                    this.logger.LogDebug(error);
                }
            }

            this.cartService.EmptyCart();
        }

        private string CreateEmaiContent(string email)
        {
            var dir = Directory.GetCurrentDirectory();

            var path = $"{dir}/wwwroot/templates/SendConfirmOrderEmail.html";

            string html = System.IO.File.ReadAllText(path);

            html = html.Replace("--Title--", $"Hello, {email}");

            html = html.Replace("--Content1--", "Thank you for choosing us! Hope you enjoy our cakes!");

            html = html.Replace("--Content2--", "Your order is accepted. Our staff members will contact you shortly.");

            string orderDetails = GetOrderDetails(this.cartService.GetCartItems());

            html = html.Replace("--Content3--", $"{orderDetails}");

            return html;
        }

        private string GetOrderDetails(CartViewModel cartItems)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"Your order: {cartItems.CartItems.First().OrderId}<br/>");

            foreach (var item in cartItems.CartItems)
            {
                sb.AppendLine($"Product: {item.Product.Name}/ Quantity: {item.Quantity}/ Price: {item.Product.Price}<br/>");
            }

            var totalValue = cartItems.TotalValue;

            sb.AppendLine($"Total value: ${totalValue}<br/>");

            return sb.ToString();
        }
    }
}
