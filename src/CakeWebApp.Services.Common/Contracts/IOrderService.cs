using CakeItWebApp.ViewModels.Orders;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CakeWebApp.Services.Common.Contracts
{
    public interface IOrderService
    {
        Task<int> CreateOrder(string userName);

        IEnumerable<AllOrdersViewModel> GetAllOrdersByUser(string userName);

        Task SetOrderDetailsId(int orderDetailsId);

        IEnumerable<OrderedProductsViewModel> GetAllItemsPerOrder(int orderId);
    }
}
