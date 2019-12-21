﻿using CakeItWebApp.ViewModels.Cart;
using System.Threading.Tasks;

namespace CakeWebApp.Services.Common.Contracts
{
    public interface ICartService
    {
        Task AddToCart(int id);

        CartViewModel GetCartItems();

        void RemoveFromCart(int id);

        void EmptyCart();
    }
}
