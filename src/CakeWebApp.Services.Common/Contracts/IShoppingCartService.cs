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
        Task AddToShoppingCart(string shoppingCartId, int id);

        ICollection<ShoppingCartItem> GetCartItems(string shoppingCartId);

        //Task RemoveFromShoppingCart(int id);

        //Task ClearShoppingCart();

        //decimal ShoppingCartTotal();

        Task MigrateCart(string shoppingCartId, string userName);

        ShoppingCartViewModel GetShoppingCart(string shoppingCartId);
    }
}
