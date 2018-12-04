using CakeItWebApp.Data;
using CakeItWebApp.Models;
using CakeItWebApp.Services.Common.Repository;
using CakeItWebApp.ViewModels.ShoppingCart;
using CakeWebApp.Services.Common.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CakeWebApp.Services.Common.CommonServices
{
    public class ShoppingCartService:IShoppingCartService
    {
        private const string CartSessionKey = "CartId";
        private readonly IServiceProvider provider;
        private readonly CakeItDbContext db;

        public ShoppingCartService(IServiceProvider provider)
        {
            this.provider = provider;
            this.db = provider.GetService<CakeItDbContext>();
        }

        public void AddToShoppingCart(string shoppingCartId, Product product, int quantity)
        {
            var shoppingCartItem =
                    db.CartItems.SingleOrDefault(
                        s => s.Product.Id == product.Id && s.ShoppingCartId == shoppingCartId);

            if (shoppingCartItem == null)
            {
                shoppingCartItem = new ShoppingCartItem
                {
                    ShoppingCartId = shoppingCartId,
                    Product = product,
                    Quantity = 1
                };

                db.CartItems.Add(shoppingCartItem);
            }
            else
            {
                shoppingCartItem.Quantity++;
            }

            db.SaveChanges();
        }

        public void ClearShoppingCart(string shoppingCartId)
        {
            var cartItems = this.db
                .CartItems
                .Where(cart => cart.ShoppingCartId == shoppingCartId);

            this.db.CartItems.RemoveRange(cartItems);

            this.db.SaveChanges();
        }

        public ICollection<ShoppingCartItem> GetCartItems(string shoppingCartId)
        {
            return this.db.CartItems.Where(c => c.ShoppingCartId == shoppingCartId)
                          .ToList();
        }

        public ShoppingCartViewModel GetShoppingCart()
        {
            ISession session = this.provider.GetRequiredService<IHttpContextAccessor>()?
                .HttpContext.Session;

            string cartId = session.GetString(CartSessionKey) ?? Guid.NewGuid().ToString();

            session.SetString(CartSessionKey, cartId);

            return new ShoppingCartViewModel() { Id = cartId };
        }

        public void RemoveFromShoppingCart(string shoppingCartId, Product product)
        {
            var shoppingCartItem =
                    this.db.CartItems.SingleOrDefault(
                        s => s.Product.Id == product.Id && s.ShoppingCartId == shoppingCartId);

            if (shoppingCartItem != null)
            {
                if (shoppingCartItem.Quantity > 1)
                {
                    shoppingCartItem.Quantity--;
                }
                else
                {
                    this.db.CartItems.Remove(shoppingCartItem);
                }
            }

            this.db.SaveChanges();
        }

        public decimal ShoppingCartTotal(string shoppingCartId)
        {
            return this.db.CartItems.Where(c => c.ShoppingCartId == shoppingCartId)
               .Select(c => c.Product.Price * c.Quantity).Sum();
        }

        public void MigrateCart(string shoppingCartId, string userName)
        {
            var shoppingCart = this.db.CartItems.Where(c => c.ShoppingCartId == shoppingCartId);

            foreach (var item in shoppingCart)
            {
                item.ShoppingCartId = userName;
            }

            ISession session = this.provider.GetService<IHttpContextAccessor>().HttpContext.Session;

            session.SetString(CartSessionKey, userName);

            this.db.SaveChanges();
        }
    }
}
