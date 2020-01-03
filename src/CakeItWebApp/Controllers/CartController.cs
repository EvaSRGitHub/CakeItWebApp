using CakeItWebApp.ViewModels.Orders;
using CakeWebApp.Services.Common.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace CakeItWebApp.Controllers
{
    public class CartController : Controller
    {
        private readonly ILogger<CartController> logger;
        private readonly ICartService cartService;
        private readonly IEmailService emailService;

        public CartController(ILogger<CartController> logger, ICartService cartService, IEmailService emailService)
        {
            this.logger = logger;
            this.cartService = cartService;
            this.emailService = emailService;
        }

        public IActionResult Index()
        {
            var model = this.cartService.GetCartItems();

            return View(model);
        }

        public async Task<IActionResult> AddToCart(int id)
        {
            try
            {
                await this.cartService.AddToCart(id);
            }
            catch (Exception e)
            {
                ViewData["Errors"] = e.Message;

                return this.View("Error");
            }

            return RedirectToAction("Index", "Cart");
        }

        public async Task<IActionResult> RemoveItem(int id)
        {
            try
            {
                await this.cartService.RemoveFromCart(id);
            }
            catch (Exception e)
            {
                ViewData["Errors"] = e.Message;

                return this.View("Error");
            }

            return RedirectToAction("Index", "Cart");
        }
    }
}
