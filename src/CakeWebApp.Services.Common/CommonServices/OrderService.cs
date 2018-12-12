using CakeItWebApp.Models;
using CakeItWebApp.Services.Common.Repository;
using CakeItWebApp.ViewModels.Orders;
using CakeWebApp.Services.Common.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CakeWebApp.Services.Common.CommonServices
{
    public class OrderService : IOrderService
    {
        private readonly IRepository<Order> repository;

        public OrderService(IRepository<Order> repository)
        {
            this.repository = repository;
        }

        public Task Checkout(OrderDetailsViewModel model)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<OrderDetailsViewModel> GetAllOrdersByUser(string username)
        {
            throw new NotImplementedException();
        }
    }
}
