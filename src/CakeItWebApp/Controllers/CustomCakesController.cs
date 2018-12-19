﻿using System;
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
        private readonly IErrorService errorService;
        private readonly ILogger<CustomCakesController> logger;

        public CustomCakesController(ICustomCakeService customCakeService, ICakeService cakeService, IErrorService errorService, ILogger<CustomCakesController> logger)
        {
            this.customCakeService = customCakeService;
            this.cakeService = cakeService;
            this.errorService = errorService;
            this.logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(CustomCakeOrderViewModel model)
        {
            if (!ModelState.IsValid)
            {
                this.logger.LogError("Model is not valid.");

                var errors = this.ModelState.Values.SelectMany(p => p.Errors).Select(e => e.ErrorMessage).ToList();

               var errorModel = this.errorService.GetErrorModel(errors);

                return View("Error", errorModel);
            }

            CustomCakeOrderViewModel result = null;

            try
            {
                result = this.customCakeService.AssignImgAndPrice(model);
            }
            catch (Exception e)
            {
                ViewData["Errors"] = e.Message;

                return this.View("Error");
            }

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

                var errorModel = this.errorService.GetErrorModel(errors);

                return View("Error", errorModel);
            }


            int? customProductId = null;

            try
            {
                await this.cakeService.AddCakeToDb(product);

                customProductId = await this.customCakeService.GetProductId();
            }
            catch (Exception e)
            {
                ViewData["Errors"] = e.Message;

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

                var errorModel = this.errorService.GetErrorModel(errors);

                return View("Error", errorModel);
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
