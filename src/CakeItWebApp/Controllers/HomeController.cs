using CakeWebApp.Services.Common.Contracts;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

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

            //if (model == null)
            //{
            //    var errorMessage = "The site is under construction.Please excuse us and try again later.";

            //    ViewData["Errors"] = errorMessage;

            //    return View("Error");
            //}

            return View(model);
        }

        //[Route("Home/Error404")]
        public IActionResult Error404()
        {
            string originalPath = "unknown";

            if (HttpContext.Items.ContainsKey("originalPath"))
            {
                originalPath = HttpContext.Items["originalPath"] as string;
            }
            
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

    }
}
