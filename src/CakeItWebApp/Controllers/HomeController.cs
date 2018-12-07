using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CakeItWebApp.Models;
using CakeWebApp.Services.Common.Contracts;
using CakeItWebApp.ViewModels.Home;

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
            var rnd = new Random();

            var max = this.homeService.GetCakeProductsCount();

            if(max == 0)
            {
                var errorMessage = "The site is under construction.Temporary unavailable.";

                this.errorService.PassErrorParam(new object[] { errorMessage });

                ViewData["Errors"] = this.errorService.ErrorParm;

                return View("Error");
            }

            HomeIndexViewModel model; 

            while (true)
            {
                var cakeIdToDesplay = rnd.Next(1, max + 1);

                model = await this.homeService.GetCakeById(cakeIdToDesplay);

                if (model != null)
                {
                    break;
                }
            }
           
            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

    }
}
