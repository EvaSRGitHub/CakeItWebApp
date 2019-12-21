using CakeItWebApp.Models;
using System.Collections.Generic;
using System.Linq;

namespace CakeItWebApp.ViewModels.Cart
{
    public class CartViewModel
    {
        public CartViewModel()
        {
            this.CartItems = new List<Item>();
        }

        public ICollection<Item> CartItems { get; set; }

        public decimal TotalValue => this.CartItems.Sum(i => i.Product.Price * i.Quantity);
    }
}
