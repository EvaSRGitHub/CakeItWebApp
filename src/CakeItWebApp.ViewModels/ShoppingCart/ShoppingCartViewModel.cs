using CakeItWebApp.Models;
using System;
using System.Collections.Generic;

namespace CakeItWebApp.ViewModels.ShoppingCart
{
    public class ShoppingCartViewModel
    {
        public string Id { get; set; }

        public DateTime CreatedOn { get; set; }

        public bool IsFinished { get; set; }

        public ICollection<ShoppingCartItem> CartItems { get; set; }
    }
}
