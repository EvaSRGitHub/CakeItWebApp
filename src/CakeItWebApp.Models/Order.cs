using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CakeItWebApp.Models
{
    public class Order
    {
        public Order()
        {
            this.ShoppedItems = new HashSet<ShoppingCartItem>();
        }

        public int Id { get; set; }

        [ForeignKey("User"),Required]
        public string UserId { get; set; }

        public virtual CakeItUser User { get; set; }

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
