using AutoMapper;
using CakeItWebApp.Models;
using CakeItWebApp.Services.Common.Repository;
using CakeItWebApp.ViewModels.Orders;
using CakeWebApp.Services.Common.Cart;
using CakeWebApp.Services.Common.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CakeWebApp.Services.Common.CommonServices
{
    public class OrderService : IOrderService
    {
        private readonly IRepository<Order> repository;
        private readonly IRepository<CakeItUser> userRepo;
        private readonly ICartManager cartManager;
        private readonly IMapper mapper;

        public OrderService(IRepository<Order> repository, IRepository<CakeItUser> userRepo, ICartManager cartManager, IMapper mapper)
        {
            this.repository = repository;
            this.userRepo = userRepo;
            this.cartManager = cartManager;
            this.mapper = mapper;
        }

        public async Task<int> CreateOrder(string userName)
        {
            var userId = this.userRepo.All().SingleOrDefault(u => u.UserName == userName)?.Id ?? null;

            var orderedItems = this.cartManager.GetCartItem();

            if (orderedItems.Count() == 0)
            {
                throw new InvalidOperationException("Your shopping cart is empty.");
            }

            var model = new OrderViewModel
            {
                UserId = userId,
                Total = orderedItems.Sum(i => i.Product.Price * i.Quantity),
                Products = orderedItems.Select(i => new OrderProduct { ProductId = i.Product.Id, Quantity = i.Quantity }).ToList()
            };

            var order = this.mapper.Map<OrderViewModel, Order>(model);

            this.repository.Add(order);

            await this.repository.SaveChangesAsync();

            var orderId = order.Id;

            foreach (var item in orderedItems)
            {
                item.OrderId = orderId;
            }

            this.cartManager.SetCartItem(orderedItems);

            return orderId;
        }

        public IEnumerable<AllOrdersViewModel> GetAllOrdersByUser(string userName)
        {
            var orders = this.repository.All().Where(o => o.User.UserName == userName);

            var model = orders.Select(o => new AllOrdersViewModel
            {
                OrderId = o.Id,
                OrderDate = o.OrederDate.ToString("dd-MM-yyyy HH:mm"),
                Customer = userName,
                Address = o.OrderDetails.Address,
                City = o.OrderDetails.City,
                TotalValue = o.Total
            }).OrderByDescending(o => o.OrderId).ToList();

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

            var model = order.Products.Select(p => new OrderedProductsViewModel
            {
                OrderId = order.Id,
                ProductName = p.Product.Name,
                ProductPrice = p.Product.Price,
                ProductQuantity = p.Quantity
            }).ToList();

            return model;
        }
    }
}
