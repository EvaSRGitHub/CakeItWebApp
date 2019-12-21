using System.ComponentModel.DataAnnotations;

namespace CakeItWebApp.ViewModels.Orders
{
    public class OrderedProductsViewModel
    {
        [Required]
        public int OrderId { get; set; }

        [Required]
        public string ProductName { get; set; }

        [Required]
        public int ProductQuantity { get; set; }

        [Required]
        public decimal ProductPrice { get; set; }
    }
}
