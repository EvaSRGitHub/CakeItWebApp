using CakeItWebApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace CakeItWebApp.ViewModels.Orders
{
    public class OrderViewModel
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        public int? OrderDetailsId { get; set; }

        [Required]
        public decimal Total { get; set; }

        public virtual ICollection<OrderProduct> Products { get; set; }
    }
}
