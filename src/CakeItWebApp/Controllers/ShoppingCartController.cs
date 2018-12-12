﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CakeItWebApp.ViewModels.ShoppingCart;
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

        public IActionResult Index()
        {
            var model = this.shoppingCartService.GetShoppingCart();

            return View(model);
        }

        public async Task <IActionResult> AddToCart(int id)
        {
            await this.shoppingCartService.AddToShoppingCart(id);

            return RedirectToAction("Index", "ShoppingCart");
        }

        public async Task<IActionResult> RemoveItem(int id)
        {
            await this.shoppingCartService.RemoveFromShoppingCart(id);

            return RedirectToAction("Index", "ShoppingCart");
        }

        public IActionResult Checkout()
        {
            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(CheckoutViewModel model)
        {
            await this.shoppingCartService.Checkout(model);

            TempData["SentEmail"] = "true";

            return this.RedirectToAction("Index", "Cakes");
        }
    }
}
