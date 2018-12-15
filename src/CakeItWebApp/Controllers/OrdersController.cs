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
        private readonly IOrderDetailsService orderDetailsService;
        private readonly IErrorService errorService;

        public OrdersController(ILogger<OrdersController> logger, 
            IOrderService orderService, 
            IShoppingCartService shoppingCartService, IOrderDetailsService orderDetailsService, IErrorService errorService)
        {
            this.logger = logger;
            this.orderService = orderService;
            this.shoppingCartService = shoppingCartService;
            this.orderDetailsService = orderDetailsService;
            this.errorService = errorService;
        }

        public IActionResult Index()
        {
            ShwoMessageIfOrderHasBeenFinished();

            var model = this.orderService.GetAllOrdersByUser(this.User.Identity.Name).ToList();

            return View(model);
        }

        public IActionResult Checkout()
        {
            if(this.shoppingCartService.GetCartItems().Count == 0)
            {
                ViewData["Errors"] = "Your shopping cart is empty.";

                return this.View("Error");
            } 

            return this.View();
        }

        [HttpPost]
        public async Task <IActionResult> Checkout(OrderDetailsViewModel model)
        {
            int orderId = 0; 

            try
            {
              orderId = await this.orderService.CreateOrder(this.User.Identity.Name);
            }
            catch (Exception e)
            {
                ViewData["Errors"] = e.Message;
                return this.View("Error");
            }

            model.OrderId = orderId;

            if (!ModelState.IsValid)
            {
                this.logger.LogError("OrderDetailsViewModel is not valid.");

                var errors = this.ModelState.Values.SelectMany(p => p.Errors).Select(e => e.ErrorMessage).ToList();

                var errorModel = this.errorService.GetErrorModel(errors);

                return View("Error", errorModel);
            }
            
            int orderDetailsId = await this.orderDetailsService.AddOrderDetails(model);

            //TO make OrderOrderDetails Table!!!????
            await this.orderService.SetOrderDetailsId(orderDetailsId);

            await this.shoppingCartService.ClearShoppingCart();

            TempData["FinishedOrder"] = "true";

            return this.RedirectToAction("Index", "Orders");
        }

        public IActionResult OrderedProducts(int orderId)
        {
            var model = this.orderService.GetAllItemsPerOrder(orderId);

            return this.View(model);
        }

        private void ShwoMessageIfOrderHasBeenFinished()
        {
            var value = TempData["FinishedOrder"]?.ToString() ?? null;

            if (value == "true")
            {
                ViewData["FinishedOrder"] = "Thank you for using CakeIt. A member from our team shall contact you shortly.";
            }
        }
    }
}
