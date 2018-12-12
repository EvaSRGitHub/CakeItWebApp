using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CakeItWebApp.ViewModels.Cakes;
using CakeWebApp.Services.Common.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using X.PagedList;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CakeItWebApp.Controllers
{
    public class CakesController : Controller
    {
        private readonly ILogger<CakesController> logger;
        private readonly ICakeService cakeService;

        public CakesController(ILogger<CakesController> logger, ICakeService cakeService)
        {
            this.logger = logger;
            this.cakeService = cakeService;
        }

        // GET: /<controller>/
        public IActionResult Index(int? page)
        {
            ShwoMessageIfOrderHasBeenFinished();

            var allCakes = this.cakeService.GetAllCakes();

            var nextPage = page ?? 1;

            var cakesPerPage = allCakes.ToPagedList(nextPage, 3);

            return View(cakesPerPage);
        }

        private void ShwoMessageIfOrderHasBeenFinished()
        {
            var value = TempData["SentEmail"]?.ToString() ?? null;

            if (value == "true")
            {
                ViewData["SentEmail"] = "Your order is accepted. Please check your e-mail.";
            }
        }

        [Authorize(Roles="Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(CreateCakeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                this.logger.LogError("Model is not valid.");

                var errors = this.ModelState.Values.SelectMany(p => p.Errors).Select(e => e.ErrorMessage).ToList();

                return View("Error", errors);
            }

            var result = await this.cakeService.AddCakeToDb(model);

            if (result != "true")
            {
                return View("Error", result);
            }

            return Redirect("/");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
           var model = await this.cakeService.GetCakeById(id);
            
            if(model == null)
            {
                var errorMessage = "Product not found.";

                return this.View("Error", errorMessage);
            }

            return this.View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(EditAndDeleteViewModel model)
        {
           var successMessage = await this.cakeService.UpdateCake(model);

            if (successMessage != "true")
            {
                return this.View("Error", successMessage);
            }

            return Redirect("/Cakes/Index");
        }

        [HttpPost]
        [Authorize(Roles = "Adimn")]
        public IActionResult Delete(int id)
        {
            this.cakeService.DeleteCake(id);

            return this.View("/Cakes/Index");
        }

        [HttpPost]
        public object RateCake(int jokeId, int rating)
        {
            var rateJoke = this.cakeService.AddRatingToCake(jokeId, rating);
            if (!rateJoke)
            {
                return Json($"An error occurred while processing your vote");
            }
            var successMessage = $"You successfully rated the joke with rating of {rating}";
            return Json(successMessage);
        }
    }
}
