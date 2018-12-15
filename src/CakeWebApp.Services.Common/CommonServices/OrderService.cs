using AutoMapper;
using CakeItWebApp.Models;
using CakeItWebApp.Services.Common.Repository;
using CakeItWebApp.ViewModels.Orders;
using CakeWebApp.Services.Common.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CakeWebApp.Services.Common.CommonServices
{
    public class OrderService : IOrderService
    {
        private readonly IRepository<Order> repository;
        private readonly IRepository<CakeItUser> userRepo;
        private readonly IRepository<ShoppingCartItem> shoppingCartItems;

        private readonly IMapper mapper;

        public OrderService(IRepository<Order> repository, IRepository<CakeItUser> userRepo, IRepository<ShoppingCartItem> shoppingCartItems, IMapper mapper)
        {
            this.repository = repository;
            this.userRepo = userRepo;
            this.shoppingCartItems = shoppingCartItems;
            this.mapper = mapper;
        }

        public async Task<int> CreateOrder(string userName)
        {
            var userId = this.userRepo.All().SingleOrDefault(u => u.UserName == userName).Id;

            var orderedItems = this.shoppingCartItems.All().Where(i => i.ShoppingCartId == userName).ToList();

            if(orderedItems.Count() == 0)
            {
                throw new InvalidOperationException("Your shopping cart is empty.");
            }

            var model = new OrderViewModel
            {
                UserId = userId,
                Total = orderedItems.Sum(i => i.Product.Price * i.Quantity),
                Products = orderedItems.Select(i  => new OrderProduct { ProductId = i.ProductId, Quantity = i.Quantity }).ToList()
            };

            var order = this.mapper.Map<OrderViewModel, Order>(model);

            this.repository.Add(order);

            await this.repository.SaveChangesAsync();

            return order.Id;
        }

        public IEnumerable<AllOrdersViewModel> GetAllOrdersByUser(string userName)
        {
            var orders = this.repository.All().Where(o => o.User.UserName == userName);

            var model = orders.Select(o => new AllOrdersViewModel {
                OrderId = o.Id,
                OrderDate = o.OrederDate.ToString("dd-MM-yyyy hh:mm"),
                Customer = userName,
                Address = o.OrderDetails.Address,
                City = o.OrderDetails.City,
                TotalValue = o.Total
            }).ToList();

            return model;
        }

        public async Task SetOrderDetailsId(int orderDetailsId)
        {
            var order = this.repository.All().Last();

            order.OrderDetailsId = orderDetailsId;
            
           await this.repository.SaveChangesAsync();
        }

        public IEnumerable<OrderedProductsViewModel> GetAllItemsPerOrder(int orderId)
        {
            var order = this.repository.All().SingleOrDefault(o => o.Id == orderId);

            var model = order.Products.Select(p => new OrderedProductsViewModel {
                OrderId = order.Id,
                ProductName = p.Product.Name,
                ProductPrice = p.Product.Price,
                ProductQuantity = p.Quantity
            }).ToList();

            return model;
        }
    }
}
