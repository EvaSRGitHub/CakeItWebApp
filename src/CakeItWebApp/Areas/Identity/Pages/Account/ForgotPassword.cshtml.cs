using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using CakeItWebApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CakeItWebApp.Services.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.IO;
using CakeWebApp.Services.Common.Contracts;

namespace CakeItWebApp.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<CakeItUser> _userManager;
        private readonly ICustomEmilSender _emailSender;
        private readonly IServiceProvider _provider;

        public ForgotPasswordModel(UserManager<CakeItUser> userManager, ICustomEmilSender emailSender, IServiceProvider provider)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _provider = provider;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Input.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return RedirectToPage("./ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please 
                // visit https://go.microsoft.com/fwlink/?LinkID=532713
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Page(
                    "/Account/ResetPassword",
                    pageHandler: null,
                    values: new { code },
                    protocol: Request.Scheme);

                string content = CreateEmaiContent(Input.Email, callbackUrl);

                //if the mail is html the link shoul be HtmlEncoder.Default.Encode(callbackUrl). 
                SendEmailDetails details = new SendEmailDetails()
                {
                    FromEmail = this._provider.GetService<IConfiguration>()["VerificationEmailDetails:FromEmail"],
                    FromName = this._provider.GetService<IConfiguration>()["VerificationEmailDetails:FromName"],
                    ToEmail = Input.Email,
                    ToName = Input.Email,
                    Subject = "Reset Password",
                    Content = content
                };

                var sendEmailResult = await this._provider.GetService<ICustomEmilSender>().SendEmailAsync(details);

                if (!sendEmailResult.Successful)
                {
                    var errors = sendEmailResult.Errors;

                    this._provider.GetService<IErrorService>().PassErrorParam(errors);

                    return RedirectToPage("/Error");
                }

                return RedirectToPage("./ForgotPasswordConfirmation");
            }

            return Page();
        }
        private string CreateEmaiContent(string email, string callbackUrl)
        {
            var dir = Directory.GetCurrentDirectory();
            var path = $"{dir}/wwwroot/templates/SendConfirmEmail.html";
            string html = System.IO.File.ReadAllText(path);
            html = html.Replace("--Title--", $"Hello, {email}");
            html = html.Replace("--Content1--", "Thanky you for choosing our web site.");
            html = html.Replace("--Content2--", "To reset your password please click the button");
            html = html.Replace(@"--Url--", $"{HtmlEncoder.Default.Encode(callbackUrl)}");
            html = html.Replace("-- ButtonText--", "Reset your email");
            return html;
        }

    }
}
