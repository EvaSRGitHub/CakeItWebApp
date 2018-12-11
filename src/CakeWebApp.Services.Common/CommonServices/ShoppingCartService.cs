using CakeItWebApp.Data;
using CakeItWebApp.Models;
using CakeItWebApp.Services.Common.Repository;
using CakeItWebApp.ViewModels.ShoppingCart;
using CakeWebApp.Models;
using CakeWebApp.Services.Common.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CakeWebApp.Services.Common.CommonServices
{
    public class ShoppingCartService : IShoppingCartService
    {
        private const string CartSessionKey = "CartId";
        private readonly IServiceProvider provider;
        private readonly ILogger<ShoppingCartService> logger;
        private readonly IRepository<ShoppingCartItem> repository;

        public ShoppingCartService(IRepository<ShoppingCartItem> repository, ILogger<ShoppingCartService> logger, IServiceProvider provider)
        {
            this.repository = repository;
            this.logger = logger;
            this.provider = provider;
        }

        //public ShoppingCart ShoppingCart { get; private set; }

        public async Task AddToShoppingCart(string shoppingCartId, int id)
        {
            var shoppingCartItem =
                   this.repository.All().SingleOrDefault(s => s.Product.Id == id && s.ShoppingCartId == shoppingCartId);

            if (shoppingCartItem == null)
            {
                shoppingCartItem = new ShoppingCartItem
                {
                    ShoppingCartId = shoppingCartId,
                    ProductId = id,
                    Quantity = 1
                };

                this.repository.Add(shoppingCartItem);
            }
            else
            {
                shoppingCartItem.Quantity ++;
            }

            await this.repository.SaveChangesAsync();
        }

        public ICollection<ShoppingCartItem> GetCartItems(string shoppingCartId)
        {
            return this.repository.All().Where(c => c.ShoppingCartId == shoppingCartId)
                          .ToList();
        }

        //public async Task ClearShoppingCart()
        //{
        //    var cartItems = this.repository
        //       .All()
        //        .Where(cart => cart.ShoppingCartId == this.CartId);

        //    this.repository.DeleteRange(cartItems);

        //    await this.repository.SaveChangesAsync();
        //}

        //

        //public async Task RemoveFromShoppingCart(int id)
        //{
        //    var shoppingCartItem =
        //            this.repository.All().SingleOrDefault(
        //                s => s.Product.Id == id && s.ShoppingCartId == this.CartId);

        //    if (shoppingCartItem != null)
        //    {
        //        if (shoppingCartItem.Quantity > 1)
        //        {
        //            shoppingCartItem.Quantity--;
        //        }
        //        else
        //        {
        //            this.repository.Delete(shoppingCartItem);
        //        }
        //    }

        //    await this.repository.SaveChangesAsync();
        //}

        //public decimal ShoppingCartTotal()
        //{
        //    return this.repository.All().Where(c => c.ShoppingCartId == this.CartId)
        //       .Select(c => c.Product.Price * c.Quantity).Sum();
        //}

        public async Task MigrateCart(string shoppingCartId, string userName)
        {
            var shoppingCartItems = this.repository.All().Where(c => c.ShoppingCartId ==  shoppingCartId);

            foreach (var item in shoppingCartItems)
            {
                item.ShoppingCartId = userName;
            }

            ISession session = this.provider.GetService<IHttpContextAccessor>().HttpContext.Session;

            session.SetString(CartSessionKey, userName);

            await this.repository.SaveChangesAsync();
        }

        public ShoppingCartViewModel GetShoppingCart(string shoppingCartId)
        {
            var model = new ShoppingCartViewModel
            {
                Id = shoppingCartId,
                CartItems = this.GetCartItems(shoppingCartId)
            };

            return model;
        }
    }
}
