using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CakeItWebApp.ViewModels.Orders;
using CakeWebApp.Services.Common.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CakeItWebApp.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly ILogger<OrdersController> logger;
        private readonly IOrderService orderService;
        private readonly IShoppingCartService shoppingCartService;

        public OrdersController(ILogger<OrdersController> logger, 
            IOrderService orderService, 
            IShoppingCartService shoppingCartService)
        {
            this.logger = logger;
            this.orderService = orderService;
            this.shoppingCartService = shoppingCartService;
        }

        public IActionResult Index()
        {
            var model = this.orderService.GetAllOrdersByUser(this.User.Identity.Name);

            return View(model);
        }

        public IActionResult Checkout()
        {
            return this.View();
        }

        [HttpPost]
        public async Task <IActionResult> Checkout(OrderDetailsViewModel model)
        {
            await this.orderService.Checkout(model);

            await this.shoppingCartService.ClearShoppingCart();

            return this.RedirectToAction("Index", "Orders");
        }
    }
}
