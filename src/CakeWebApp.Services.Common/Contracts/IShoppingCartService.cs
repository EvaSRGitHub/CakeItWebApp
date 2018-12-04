using CakeItWebApp.Models;
using CakeItWebApp.ViewModels;
using CakeItWebApp.ViewModels.ShoppingCart;
using System;
using System.Collections.Generic;

namespace CakeWebApp.Services.Common.Contracts
{
    public interface IShoppingCartService
    {
        ShoppingCartViewModel GetShoppingCart();

        void AddToShoppingCart(string shoppingCartId, Product product, int quantity);

        void RemoveFromShoppingCart(string shoppingCartId, Product product);

        ICollection<ShoppingCartItem> GetCartItems(string shoppingCartId);

        void ClearShoppingCart(string shoppingCartId);

        decimal ShoppingCartTotal(string shoppingCartId);

        void MigrateCart(string shoppingCartId, string userName);
    }
}
