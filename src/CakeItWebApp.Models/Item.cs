namespace CakeItWebApp.Models
{
    public class Item
    {
        public int? OrderId { get; set; }

        public Product Product { get; set; }

        public int Quantity { get; set; }
    }
}
