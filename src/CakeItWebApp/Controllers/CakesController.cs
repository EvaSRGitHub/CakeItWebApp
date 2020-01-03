using CakeItWebApp.ViewModels.Cakes;
using CakeWebApp.Services.Common.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace CakeItWebApp.Controllers
{
    public class CakesController : Controller
    {
        private const int MaxCakesPerPage = 3;

        private readonly ILogger<CakesController> logger;
        private readonly ICakeService cakeService;
        private readonly IErrorService errorService;

        public CakesController(ILogger<CakesController> logger, ICakeService cakeService, IErrorService errorService)
        {
            this.logger = logger;
            this.cakeService = cakeService;
            this.errorService = errorService;
        }

        public IActionResult Index(int? page)
        {
            ShwoMessageIfOrderHasBeenFinished();

            var userIsAdmin = this.User.IsInRole("Admin");

            var allCakes = userIsAdmin ? this.cakeService.GetAllCakes() : this.cakeService.GetAllActiveCakes(); 

            var nextPage = page ?? 1;

            var cakesPerPage = allCakes.ToPagedList(nextPage, MaxCakesPerPage);

            return View(cakesPerPage);
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
                this.logger.LogDebug("Model is not valid.");

                var errors = this.ModelState.Values.SelectMany(p => p.Errors).Select(e => e.ErrorMessage).ToList();

                var errorModel = this.errorService.GetErrorModel(errors);

                return View("Error", errorModel);
            }

            try
            {
                await this.cakeService.AddCakeToDb<CreateCakeViewModel>(model);
            }
            catch (Exception e)
            {
                ViewData["Errors"] = e.Message;

                return View("Error");
            }

            return Redirect("/");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            EditAndDeleteViewModel model;

            try
            {
                model = await this.cakeService.GetCakeToEdit(id);
            }
            catch (Exception e)
            {
                ViewData["Errors"] = e.Message;

                return this.View("Error");
            }

            return this.View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(EditAndDeleteViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = this.ModelState.Values.SelectMany(p => p.Errors).Select(e => e.ErrorMessage).ToList();

                var errorModel = this.errorService.GetErrorModel(errors);

                return View("Error", errorModel);
            }

           var successMessage = await this.cakeService.UpdateCake(model);

            if (successMessage != "true")
            {
                return this.View("Error", successMessage);
            }

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            EditAndDeleteViewModel model;

            try
            {
              model = await this.cakeService.GetCakeById(id);
            }
            catch (Exception e)
            {
                ViewData["Errors"] = e.Message;

                return this.View("Error");
            }

            return this.View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(EditAndDeleteViewModel model)
        {
            try
            {
                //await this.cakeService.DeleteCake(model);
                await this.cakeService.SoftDelete(model.Id);
            }
            catch (Exception e)
            {
                ViewData["Errors"] = e.Message;

                return this.View("Error");
            }

            return this.RedirectToAction("Index");
        }

        public async Task<IActionResult> Details(int id)
        {
            var cake = await this.cakeService.ShowCakeDetails(id);

            if(cake == null)
            {
                ViewData["Errors"] = "Product not found!";

                return this.View("Error");
            }

            return this.View(cake);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Rate(int cakeId, int rating)
        {
            try
            {
                await this.cakeService.AddRatingToCake(cakeId, rating);
            }
            catch (Exception e)
            {
                ViewData["Еrrors"] = e.Message;

                return this.View("Error");
            }

            TempData["Rate"] = "Your rating has been successfully registered.";

            return RedirectToAction(nameof(Index));
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
