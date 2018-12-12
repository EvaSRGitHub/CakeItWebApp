using CakeItWebApp.ViewModels.Orders;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CakeWebApp.Services.Common.Contracts
{
    public interface IOrderService
    {
        IEnumerable<OrderDetailsViewModel> GetAllOrdersByUser(string username);

         Task Checkout(OrderDetailsViewModel model);
    }
}
