using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CakeItWebApp.Models
{
    public class ShoppingCartItem
    {
        public int Id { get; set; }

        public string ShoppingCartId { get; set; }

        [ForeignKey("Product"),Required]
        public int ProductId { get; set; }

        public virtual Product Product{ get; set; }

        [Required]
        public int Quantity { get; set; }
    }
}
