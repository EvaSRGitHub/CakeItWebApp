using CakeItWebApp.Models;
using CakeItWebApp.ViewModels;
using CakeItWebApp.ViewModels.ShoppingCart;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CakeWebApp.Models;

namespace CakeWebApp.Services.Common.Contracts
{
    public interface IShoppingCartService
    {
        Task AddToShoppingCart(int id);

        ICollection<ShoppingCartItem> GetCartItems();

        Task RemoveFromShoppingCart(int id);

        Task ClearShoppingCart();

        Task MigrateCart(string userName);

        ShoppingCartViewModel GetShoppingCart();

        Task Checkout(CheckoutViewModel model);
    }
}
