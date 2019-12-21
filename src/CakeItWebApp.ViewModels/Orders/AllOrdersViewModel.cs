namespace CakeItWebApp.ViewModels.Orders
{
    public class AllOrdersViewModel
    {
        public int OrderId { get; set; }

        public string OrderDate { get; set; }

        public string Customer { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public decimal TotalValue { get; set; }
    }
}
