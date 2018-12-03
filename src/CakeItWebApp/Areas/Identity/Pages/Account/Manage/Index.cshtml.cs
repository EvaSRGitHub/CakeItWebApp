using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using CakeItWebApp.Models;
using CakeItWebApp.Services.Messaging;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;
using CakeWebApp.Services.Common.Contracts;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace CakeItWebApp.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<CakeItUser> _userManager;
        private readonly SignInManager<CakeItUser> _signInManager;
        private readonly IServiceProvider _provider;

        public IndexModel(
            UserManager<CakeItUser> userManager,
            SignInManager<CakeItUser> signInManager,
           IServiceProvider provider)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _provider = provider;
        }

        public string Username { get; set; }

        public bool IsEmailConfirmed { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var userName = await _userManager.GetUserNameAsync(user);
            var email = await _userManager.GetEmailAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;

            Input = new InputModel
            {
                Email = email,
                PhoneNumber = phoneNumber
            };

            IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var email = await _userManager.GetEmailAsync(user);
            if (Input.Email != email)
            {
                var setEmailResult = await _userManager.SetEmailAsync(user, Input.Email);
                if (!setEmailResult.Succeeded)
                {
                    var userId = await _userManager.GetUserIdAsync(user);
                    throw new InvalidOperationException($"Unexpected error occurred setting email for user with ID '{userId}'.");
                }
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    var userId = await _userManager.GetUserIdAsync(user);
                    throw new InvalidOperationException($"Unexpected error occurred setting phone number for user with ID '{userId}'.");
                }
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostSendVerificationEmailAsync()
        {
            if (!ModelState.IsValid)
            {
                var errors = this.ModelState.Values.SelectMany(p => p.Errors).Select(e => e.ErrorMessage).ToArray();

                this._provider.GetService<IErrorService>().PassErrorParam(errors);

                return RedirectToPage("/Error");
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }


            var userId = await _userManager.GetUserIdAsync(user);
            var email = await _userManager.GetEmailAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { userId = userId, code = code },
                protocol: Request.Scheme);

            string content = CreateEmaiContent(Input.Email, callbackUrl);

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

            StatusMessage = "Verification email sent. Please check your email.";

            return RedirectToPage();
        }

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
