using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CakeItWebApp.Models
{
    public class Order
    {
        public Order()
        {
            this.OrederDate = DateTime.UtcNow;
            this.Products = new HashSet<OrderProduct>();
        }

        public int Id { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }

        public virtual CakeItUser User { get; set; }

        [Required]
        public DateTime OrederDate { get; set; }

        [ForeignKey("OrderDetails")]
        public int OrderDetailsId { get; set; }

        public virtual OrderDetails OrderDetails { get; set; }

        [Required]
        public decimal Total { get; set; }

        public virtual ICollection<OrderProduct> Products { get; set; }
    }
}
