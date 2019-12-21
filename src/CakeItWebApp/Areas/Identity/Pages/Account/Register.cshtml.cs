using CakeItWebApp.Models;
using CakeItWebApp.Services.Messaging;
using CakeWebApp.Services.Common.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace CakeItWebApp.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<CakeItUser> _signInManager;
        private readonly UserManager<CakeItUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IServiceProvider _provider;
        

        public RegisterModel(
            UserManager<CakeItUser> userManager,
            SignInManager<CakeItUser> signInManager,
            ILogger<RegisterModel> logger,
           IServiceProvider provider)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _provider = provider;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(10, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 3)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public IActionResult OnGet(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            if (this.User.Identity.IsAuthenticated)
            {
                return LocalRedirect(returnUrl);
            }

            ReturnUrl = returnUrl;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (ModelState.IsValid)
            {
                var user = new CakeItUser { UserName = Input.Email, Email = Input.Email };
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    IdentityResult asigneRoleResult = await _userManager.AddToRoleAsync(user, "User");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { userId = user.Id, code = code },
                        protocol: Request.Scheme);

                    string content =  CreateEmaiContent(Input.Email, callbackUrl);

                    SendEmailDetails details = new SendEmailDetails()
                    {
                        FromEmail = this._provider.GetService<IConfiguration>()["VerificationEmailDetails:FromEmail"],
                        FromName = this._provider.GetService<IConfiguration>()["VerificationEmailDetails:FromName"],
                        ToEmail = Input.Email,
                        ToName = Input.Email,
                        Subject = "Confirm your email.",
                        Content = content
                    };

                    var sendEmailResult = await this._provider.GetService<ICustomEmilSender>().SendEmailAsync(details);

                    if (!sendEmailResult.Successful)
                    {
                        var mailErrors = sendEmailResult.Errors;

                        this._provider.GetService<IErrorService>().PassErrorParam(mailErrors);

                        return RedirectToPage("/Error");
                    }

                    await _signInManager.SignInAsync(user, isPersistent: false);

                    return LocalRedirect(returnUrl);
                }

                var errors = this.ModelState.Values.SelectMany(p => p.Errors).Select(e => e.ErrorMessage).ToArray();

                this._provider.GetService<IErrorService>().PassErrorParam(errors);

                return RedirectToPage("/Error");
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        //if the mail is plane text the link shouln't be HtmlEncoded. 
        private string CreateEmaiContent(string email, string callbackUrl)
        {
            var dir = Directory.GetCurrentDirectory();
            var path = $"{dir}/wwwroot/templates/SendConfirmEmail.html";
            string html = System.IO.File.ReadAllText(path);
            html = html.Replace("--Title--", $"Hello, {email}");
            html = html.Replace("--Content1--", "Thanky you for choosing our web site. Hope enjoy it!");
            html = html.Replace("--Content2--", "Please confirm your account by clicking the button");
            html = html.Replace(@"--Url--", $"{HtmlEncoder.Default.Encode(callbackUrl)}");
            html = html.Replace("-- ButtonText--", "Verify your email");
            return html;
        }

    }
}
