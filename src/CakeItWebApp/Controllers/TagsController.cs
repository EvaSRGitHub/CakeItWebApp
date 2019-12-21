using CakeItWebApp.ViewModels.Tags;
using CakeWebApp.Services.Common.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;


namespace CakeItWebApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class TagsController : Controller
    {
        private readonly IErrorService errorService;
        private readonly ITagService tagService;

        public TagsController(IErrorService errorService, ITagService tagService)
        {
            this.errorService = errorService;
            this.tagService = tagService;
        }

        public IActionResult Index()
        {
            var allTags = this.tagService.GetAllTags().ToList();

            return View(allTags);
        }

        public IActionResult CreateTag()
        {
            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateTag(TagInputViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = this.ModelState.Values.SelectMany(p => p.Errors).Select(e => e.ErrorMessage).ToList();

                var errorModel = this.errorService.GetErrorModel(errors);

                return View("Error", errorModel);
            }

            try
            {
                await this.tagService.CreateTag(model);
            }
            catch (Exception e)
            {
                ViewData["Errors"] = e.Message;
                return this.View("Error");
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            TagInputViewModel model;

            try
            {
                model = await this.tagService.GetTagById(id);
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
        public async Task<IActionResult> Edit(TagInputViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = this.ModelState.Values.SelectMany(p => p.Errors).Select(e => e.ErrorMessage).ToList();

                var errorModel = this.errorService.GetErrorModel(errors);

                return View("Error", errorModel);
            }

            try

            {
                await this.tagService.UpdateTag(model);
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
