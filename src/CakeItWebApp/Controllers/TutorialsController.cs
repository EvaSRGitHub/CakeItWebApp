using CakeItWebApp.ViewModels.Tutorials;
using CakeWebApp.Services.Common.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace CakeItWebApp.Controllers
{
    [Authorize]
    public class TutorialsController : Controller
    {
        private readonly ITutorialService tutorialService;
        private readonly IErrorService errorService;

        public TutorialsController(ITutorialService tutorialService, IErrorService errorService)
        {
            this.tutorialService = tutorialService;
            this.errorService = errorService;
        }

        public IActionResult Index(int? page)
        {
            var allTutorials = this.tutorialService.GetTutorials();

            var nextPage = page ?? 1;

            var cakesPerPage = allTutorials.ToPagedList(nextPage, 3);

            return View(cakesPerPage);
        }

        [Authorize(Roles="Admin")]
        public IActionResult AddTutorial()
        {
            return this.View();
        }

        [HttpPost]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> AddTutorial(AddTutorialViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = this.ModelState.Values.SelectMany(p => p.Errors).Select(e => e.ErrorMessage).ToList();

                var errorModel = this.errorService.GetErrorModel(errors);

                return View("Error", errorModel);
            }

            await this.tutorialService.AddTutorial(model);

            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Rate(int tutorialId, int rating)
        {
            try
            {
                await this.tutorialService.AddRatingToTutorial(tutorialId, rating);
            }
            catch (Exception e)
            {
                ViewData["Еrrors"] = e.Message;

                return this.View("Error");
            }

            TempData["Rate"] = "Your rating has been successfully registered.";

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            AddTutorialViewModel model;

            try
            {
               model = await this.tutorialService.GetTutorialById(id);
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
        public async Task<IActionResult> Edit(AddTutorialViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = this.ModelState.Values.SelectMany(p => p.Errors).Select(e => e.ErrorMessage).ToList();

                var errorModel = this.errorService.GetErrorModel(errors);

                return View("Error", errorModel);
            }

            try

            {
                await this.tutorialService.UpdateTutorial(model);
            }
            catch (Exception e)
            {
                ViewData["Errors"] = e.Message;

                return this.View("Error");
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await this.tutorialService.DeleteTutorial(id);
            }
            catch (Exception e)
            {
                ViewData["Errors"] = e.Message;

                return this.View("Error");
            }

            return RedirectToAction("Index");
        }
    }
}
