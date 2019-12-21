using CakeItWebApp.ViewModels.Orders;
using CakeWebApp.Services.Common.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CakeItWebApp.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ILogger<OrdersController> logger;
        private readonly IOrderService orderService;
        private readonly ICartService cartService;
        private readonly IOrderDetailsService orderDetailsService;
        private readonly IErrorService errorService;
        private readonly ICakeService cakeService;
        private readonly IEmailService emailService;

        public OrdersController(ILogger<OrdersController> logger,
            IOrderService orderService,
            ICartService cartService, IOrderDetailsService orderDetailsService, IErrorService errorService, ICakeService cakeService, IEmailService emailService)
        {
            this.logger = logger;
            this.orderService = orderService;
            this.cartService = cartService;
            this.orderDetailsService = orderDetailsService;
            this.errorService = errorService;
            this.cakeService = cakeService;
            this.emailService = emailService;
        }

        [Authorize]
        public IActionResult Index()
        {
            ShwoMessageIfOrderHasBeenFinished();

            var model = this.orderService.GetAllOrdersByUser(this.User.Identity.Name).ToList();

            return View(model);
        }

        public IActionResult Checkout()
        {
            var items = this.cartService.GetCartItems().CartItems;

            if (items.Count == 0)
            {
                ViewData["Errors"] = "Your shopping cart is empty.";

                return this.View("Error");
            }

            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(OrderDetailsViewModel model)
        {
            int orderId = 0;

            try
            {
                orderId = await this.orderService.CreateOrder(this?.User.Identity.Name ?? null);
            }
            catch (Exception e)
            {
                ViewData["Errors"] = e.Message;
                return this.View("Error");
            }

            model.OrderId = orderId;

            if (!ModelState.IsValid)
            {
                this.logger.LogDebug("OrderDetailsViewModel is not valid.");

                var errors = this.ModelState.Values.SelectMany(p => p.Errors).Select(e => e.ErrorMessage).ToList();

                var errorModel = this.errorService.GetErrorModel(errors);

                return View("Error", errorModel);
            }

            int orderDetailsId = await this.orderDetailsService.AddOrderDetails(model);

            await this.orderService.SetOrderDetailsId(orderDetailsId);

            await MarkAsDeletedCustomProductsFromDb();

            await this.emailService.SendOrderMail(model);

            TempData["SentEmail"] = "true";

            if (!this.User.Identity.IsAuthenticated)
            {
                return this.RedirectToAction("Index", "Cakes");
            }

            return this.RedirectToAction("Index", "Orders");
        }

        private async Task MarkAsDeletedCustomProductsFromDb()
        {
            var customProduct = this.cartService.GetCartItems().CartItems.Where(i => i.Product.CategoryId == 2);

            foreach (var item in customProduct)
            {
                await this.cakeService.SoftDelete(item.Product.Id);
            }
        }

        [Authorize]
        public IActionResult OrderedProducts(int orderId)
        {
            var model = this.orderService.GetAllItemsPerOrder(orderId);

            return this.View(model);
        }

        private void ShwoMessageIfOrderHasBeenFinished()
        {
            var value = TempData["SentEmail"]?.ToString() ?? null;

            if (value == "true")
            {
                ViewData["SentEmail"] = "Your order is accepted. Please check your e-mail.";
            }
        }
    }
}
