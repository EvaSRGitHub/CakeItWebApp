using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using CakeWebApp.Services.Common.Contracts;

namespace CakeItWebApp.Areas.Identity.Pages
{
    [AllowAnonymous]
    public class ErrorModel : PageModel
    {
        private readonly IErrorService errorService;

        public ErrorModel(IErrorService errorService)
        {
            this.errorService = errorService;
            this.Errors = new List<string>();
        }

        [BindProperty(SupportsGet = true)]
        public ICollection<string> Errors { get; set; } = new List<string>();

        public void OnGet()
        {
            Errors = errorService.ErrorParm as List<string>;
        }
    }
}