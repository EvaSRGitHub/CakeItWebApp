using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CakeItWebApp.Models;
using CakeWebApp.Services.Common.Contracts;
using CakeItWebApp.ViewModels;

namespace CakeItWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHomeService homeService;
        private readonly IErrorService errorService;

        public HomeController(IHomeService homeService, IErrorService errorService)
        {
            this.homeService = homeService;
            this.errorService = errorService;
        }

        public async Task<IActionResult> Index()
        {
           var model = await this.homeService.GetRandomCake();

            if (model == null)
            {
                var errorMessage = "The site is under construction.Please excuse us and try again later.";

                ViewData["Errors"] = errorMessage;

                return View("Error");
            }

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

    }
}
