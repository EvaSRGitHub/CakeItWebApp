using CakeItWebApp.Data;
using CakeItWebApp.Models;
using CakeItWebApp.Services.Common.Repository;
using CakeItWebApp.Services.Messaging;
using CakeItWebApp.ViewModels.ShoppingCart;
using CakeWebApp.Models;
using CakeWebApp.Services.Common.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace CakeWebApp.Services.Common.CommonServices
{
    public class ShoppingCartService : IShoppingCartService
    {
        private const string CartSessionKey = "CartId";
        private readonly IServiceProvider provider;
        private readonly ILogger<ShoppingCartService> logger;
        private readonly IRepository<ShoppingCartItem> repository;
        private readonly ShoppingCart shoppingCart;

        public ShoppingCartService(IRepository<ShoppingCartItem> repository, ILogger<ShoppingCartService> logger, IServiceProvider provider, ShoppingCart shoppingCart)
        {
            this.repository = repository;
            this.logger = logger;
            this.provider = provider;
            this.shoppingCart = shoppingCart;
        }

        public async Task AddToShoppingCart(int id)
        {
            var shoppingCartItem =
                   this.repository.All().SingleOrDefault(s => s.Product.Id == id && s.ShoppingCartId == this.shoppingCart.Id);

            if (shoppingCartItem == null)
            {
                shoppingCartItem = new ShoppingCartItem
                {
                    ShoppingCartId = shoppingCart.Id,
                    ProductId = id,
                    Quantity = 1
                };

                this.repository.Add(shoppingCartItem);
            }
            else
            {
                shoppingCartItem.Quantity++;
            }

            await this.repository.SaveChangesAsync();
        }

        public ICollection<ShoppingCartItem> GetCartItems()
        {
            return this.repository.All().Where(c => c.ShoppingCartId == this.shoppingCart.Id)
                          .ToList();
        }

        public async Task ClearShoppingCart()
        {
            var cartItems = this.repository
               .All()
                .Where(cart => cart.ShoppingCartId == this.shoppingCart.Id);

            this.repository.DeleteRange(cartItems);

            await this.repository.SaveChangesAsync();
        }

        public async Task RemoveFromShoppingCart(int id)
        {
            var shoppingCartItem =
                    this.repository.All().SingleOrDefault(
                        s => s.Product.Id == id && s.ShoppingCartId == this.shoppingCart.Id);

            if (shoppingCartItem != null)
            {
                if (shoppingCartItem.Quantity > 1)
                {
                    shoppingCartItem.Quantity--;
                }
                else
                {
                    this.repository.Delete(shoppingCartItem);
                }
            }
            await this.repository.SaveChangesAsync();
        }

        public async Task MigrateCart(string userName)
        {
            var shoppingCartItems = this.repository.All().Where(c => c.ShoppingCartId ==  this.shoppingCart.Id);

            foreach (var item in shoppingCartItems)
            {
                item.ShoppingCartId = userName;
            }

            ISession session = this.provider.GetService<IHttpContextAccessor>().HttpContext.Session;

            session.SetString(CartSessionKey, userName);

            await this.repository.SaveChangesAsync();
        }

        public ShoppingCartViewModel GetShoppingCart()
        {
            var model = new ShoppingCartViewModel
            {
                Id = this.shoppingCart.Id,
                CartItems = this.GetCartItems()
            };

            return model;
        }

        public async Task Checkout(CheckoutViewModel model)
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

            await this.ClearShoppingCart();
        }

        private string CreateEmaiContent(string email)
        {
            var dir = Directory.GetCurrentDirectory();
            var path = $"{dir}/wwwroot/templates/SendConfirmOrderEmail.html";
            string html = System.IO.File.ReadAllText(path);
            html = html.Replace("--Title--", $"Hello, {email}");
            html = html.Replace("--Content1--", "Thanky you for choosing our web site. Hope enjoy our cakes!");
            html = html.Replace("--Content2--", "Your order is accepted. Our staff members will contact you shortly.");
            var cartItems = this.GetCartItems();
            string orderDetails = GetOrderDetails(cartItems);
            
            html = html.Replace("--Content3--", $"{orderDetails}");
            //html = html.Replace(@"--Url--", $"{HtmlEncoder.Default.Encode(callbackUrl)}");
           // html = html.Replace("-- ButtonText--", "Verify your email");
            return html;
        }

        private string GetOrderDetails(ICollection<ShoppingCartItem> shoppingCart)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("You order:<br/>");
            foreach (var item in shoppingCart)
            {
                sb.AppendLine($"Product: {item.Product.Name}/ Quantity: {item.Quantity}/ Price: {item.Product.Price}<br/>");
            }

            var totalValue = shoppingCart.Sum(i => i.Product.Price * i.Quantity);

            sb.AppendLine($"Total value: ${totalValue}<br/>");

            return sb.ToString();
        }
    }
}
