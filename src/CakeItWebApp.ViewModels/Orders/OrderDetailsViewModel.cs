using CakeItWebApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CakeItWebApp.ViewModels.Orders
{
    public class OrderDetailsViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public DateTime OrederDate { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public decimal Total { get; set; }

        public virtual ICollection<ShoppingCartItem> ShoppedItems { get; set; }
    }
}
