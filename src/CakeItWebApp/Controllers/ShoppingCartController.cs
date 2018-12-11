using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CakeWebApp.Models;
using CakeWebApp.Services.Common.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CakeItWebApp.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly ILogger<ShoppingCartController> logger;
        private readonly IShoppingCartService shoppingCartService;
        private readonly ShoppingCart shoppingCart;

        public ShoppingCartController(ILogger<ShoppingCartController> logger, IShoppingCartService shoppingCartService, ShoppingCart shoppingCart)
        {
            this.logger = logger;
            this.shoppingCartService = shoppingCartService;
            this.shoppingCart = shoppingCart;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            var model = this.shoppingCartService.GetShoppingCart(shoppingCart.Id);

            return View(model);
        }

        public async Task <IActionResult> AddToCart(int id)
        {
            await this.shoppingCartService.AddToShoppingCart(shoppingCart.Id, id);

            return Redirect("/ShoppingCart/Index");
        }
    }
}
