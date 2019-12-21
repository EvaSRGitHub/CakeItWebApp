using CakeItWebApp.ViewModels.Orders;
using System.Threading.Tasks;

namespace CakeWebApp.Services.Common.Contracts
{
    public interface IOrderDetailsService
    {
        Task<int> AddOrderDetails(OrderDetailsViewModel model);
    }
}
