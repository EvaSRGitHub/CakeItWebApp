using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CakeItWebApp.ViewModels.Cakes;
using CakeWebApp.Services.Common.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

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
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles="Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCakeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                this.logger.LogError("Model is not valid.");

                var errors = this.ModelState.Values.SelectMany(p => p.Errors).Select(e => e.ErrorMessage).ToList();

                return View("Errors", errors);
            }

            var result = await this.cakeService.AddCakeToDb(model);

            if (!result)
            {
                return View();
            }

            return Redirect("/");
        }

        [HttpPost]
        public object RateCake(int jokeId, int rating)
        {
            var rateJoke = this.cakeService.AddRatingToJoke(jokeId, rating);
            if (!rateJoke)
            {
                return Json($"An error occurred while processing your vote");
            }
            var successMessage = $"You successfully rated the joke with rating of {rating}";
            return Json(successMessage);
        }
    }
}
