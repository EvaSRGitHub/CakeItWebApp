using AutoMapper;
using CakeItWebApp.Models;
using CakeItWebApp.Services.Common.Repository;
using CakeItWebApp.ViewModels.Orders;
using CakeWebApp.Services.Common.Contracts;
using System.Threading.Tasks;

namespace CakeWebApp.Services.Common.CommonServices
{
    public class OrderDetailsService : IOrderDetailsService
    {
        private readonly IRepository<OrderDetails> repository;
        private readonly IMapper mapper;

        public OrderDetailsService(IRepository<OrderDetails> repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        public async Task<int> AddOrderDetails(OrderDetailsViewModel model)
        {
            var orderDetails = this.mapper.Map<OrderDetailsViewModel, OrderDetails>(model);

            this.repository.Add(orderDetails);

            await this.repository.SaveChangesAsync();

            return orderDetails.Id;
        }
    }
}
