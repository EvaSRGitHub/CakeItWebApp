using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CakeItWebApp.Models;
using CakeItWebApp.ViewModels.CustomCake;
using CakeWebApp.Services.Common.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CakeItWebApp.Controllers
{
    [Authorize]
    public class CustomCakesController : Controller
    {
        private readonly ICustomCakeService customCakeService;
        private readonly ICakeService cakeService;
        private readonly ILogger<CustomCakesController> logger;

        public CustomCakesController(ICustomCakeService customCakeService, ICakeService cakeService, ILogger<CustomCakesController> logger)
        {
            this.customCakeService = customCakeService;
            this.cakeService = cakeService;
            this.logger = logger;
        }

        public IActionResult Index()
        {
            if (!this.User.Identity.IsAuthenticated)
            {
                return RedirectToPage("Login");
            }
            return View();
        }

        [HttpPost]
        public IActionResult Index(CustomCakeOrderViewModel model)
        {
            var result = this.customCakeService.CalculatePrice(model);

            return this.View("CustomCakeDetails", result);
        }

        public async Task<IActionResult> AddToCart()
        {
            CustomCakeOrderViewModel model = null;

            if (TempData["OrderCustomCake"] != null)
            {
               model = JsonConvert.DeserializeObject<CustomCakeOrderViewModel>(TempData["OrderCustomCake"].ToString());
            }

            Product product = null;

            try
            {
               product = this.customCakeService.CreateCustomProduct(model);
            }
            catch (Exception e)
            {
                ViewData["Errors"] = e.Message;

                return this.View("Error");
            }

            if (!ModelState.IsValid)
            {
                this.logger.LogError("Model is not valid.");

                var errors = this.ModelState.Values.SelectMany(p => p.Errors).Select(e => e.ErrorMessage).ToList();

                return View("Error", errors);
            }

            try
            {
               await this.cakeService.AddCakeToDb(product);
            }
            catch (Exception e)
            {
                ViewData["Errors"] = e.Message;

                return this.View("Error");
            }

            int? customProductId = await this.customCakeService.GetProductId();

            if(customProductId == null)
            {
                ViewData["Errors"] = "Product not Found";

                return this.View("Error");
            }

            return RedirectToAction("AddToCart", "ShoppingCart", new { Id = customProductId });
        }   
        
        [Authorize(Roles = "Admin")]
        public IActionResult AddCustomCakeImg()
        {
            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> AddCustomCakeImg(CustomCakeImgViewModel model)
        {
            if (!ModelState.IsValid)
            {
                this.logger.LogError("Model is not valid.");

                var errors = this.ModelState.Values.SelectMany(p => p.Errors).Select(e => e.ErrorMessage).ToList();

                return View("Error", errors);
            }

            try
            {
                await this.customCakeService.AddCustomCakeImg(model);
            }
            catch (Exception e)
            {
                ViewData["Errors"] = e.InnerException.Message;

                return this.View("Error");
            }

            return RedirectToAction(nameof(AllCustomCakeImg));
        }

        [Authorize(Roles = "Admin")]
        public IActionResult AllCustomCakeImg()
        {
            return this.View();
        }
    }
}
