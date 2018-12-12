using CakeItWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CakeItWebApp.ViewModels.ShoppingCart
{
    public class ShoppingCartViewModel
    {
        public string Id { get; set; }

        public ICollection<ShoppingCartItem> CartItems { get; set; }

        public decimal TotalValue => this.CartItems.Sum(i => i.Product.Price * i.Quantity);
    }
}
