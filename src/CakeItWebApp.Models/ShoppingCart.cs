using CakeItWebApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace CakeWebApp.Models
{
    public class ShoppingCart
    {
        private const string CartSessionKey = "CartId";

        public ShoppingCart()
        {
            this.IsFinished = false;
            this.CartItems = new HashSet<ShoppingCartItem>();
        }

        public string Id { get; set; }

        public bool IsFinished { get; set; }

        public ICollection<ShoppingCartItem> CartItems { get; set; }

        public static ShoppingCart GetShoppingCart(IServiceProvider provider)
        {
            ISession session = provider.GetRequiredService<IHttpContextAccessor>()?
                .HttpContext.Session;

            string cartId = session?.GetString(CartSessionKey) ?? Guid.NewGuid().ToString();

            session.SetString(CartSessionKey, cartId);

            return new ShoppingCart() { Id = cartId };
        }
    }
}
