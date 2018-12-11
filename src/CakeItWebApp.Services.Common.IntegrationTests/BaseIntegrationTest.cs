using CakeItWebApp.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xunit;

namespace CakeItWebApp.Services.Common.IntegrationTests
{
    public abstract class BaseIntegrationTest : IClassFixture<WebApplicationFactory<Startup>>
    {
        protected SetCookieHeaderValue antiforgeryCookie;

        protected string antiforgeryToken;

        protected SetCookieHeaderValue authenticationCookie;

        protected BaseIntegrationTest(WebApplicationFactory<Startup> server)
        {
            this.Server = server;
            this.Client = this.Server.CreateClient();
        }

        protected WebApplicationFactory<Startup> Server { get; private set; }

        public HttpClient Client { get; protected set; }

        protected static Regex AntiforgeryFormFieldRegex = new Regex(@"\<input name=""__RequestVerificationToken"" type=""hidden"" value=""([^""]+)"" \/\>");

        protected async Task<string> EnsureAntiforgeryToken()
        {
            if (antiforgeryToken != null) return antiforgeryToken;

            var response = await this.Client.GetAsync("/Identity/Account/Login");

            response.EnsureSuccessStatusCode();

            if (response.Headers.TryGetValues("Set-Cookie", out IEnumerable<string> values))
            {
                antiforgeryCookie = SetCookieHeaderValue.ParseList(values.ToList()).SingleOrDefault(c => c.Name.StartsWith(".AspNetCore.AntiForgery.", StringComparison.InvariantCultureIgnoreCase));
            }

            Assert.NotNull(antiforgeryCookie);

            this.Client.DefaultRequestHeaders.Add("Cookie", new CookieHeaderValue(antiforgeryCookie.Name, antiforgeryCookie.Value).ToString());

            var responseHtml = await response.Content.ReadAsStringAsync();

            var match = AntiforgeryFormFieldRegex.Match(responseHtml);

            antiforgeryToken = match.Success ? match.Groups[1].Captures[0].Value : null;

            Assert.NotNull(antiforgeryToken);

            return antiforgeryToken;
        }

        protected async Task<Dictionary<string, string>> EnsureAntiforgeryTokenForm(Dictionary<string, string> formData = null)
        {
            if (formData == null) formData = new Dictionary<string, string>();

            formData.Add("__RequestVerificationToken", await EnsureAntiforgeryToken());
            return formData;
        }

        public async Task EnsureAdminAuthenticationCookie()
        {
            if (authenticationCookie != null) return;

            var formData = await EnsureAntiforgeryTokenForm(new Dictionary<string, string>
    {
        { "Email","eva@abv.bg" },
        { "Password", "123" }
    });
           
            var response = await this.Client.PostAsync("/Identity/Account/Login", new FormUrlEncodedContent(formData));

            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);

            if (response.Headers.TryGetValues("Set-Cookie", out IEnumerable<string> values))
            {
                authenticationCookie = SetCookieHeaderValue.ParseList(values.ToList()).SingleOrDefault(c => c.Name.StartsWith(".AspNetCore.Identity.Application", StringComparison.InvariantCultureIgnoreCase));
            }

            Assert.NotNull(authenticationCookie);

           this.Client.DefaultRequestHeaders.Add("Cookie", new CookieHeaderValue(authenticationCookie.Name, authenticationCookie.Value).ToString());

            // The current pair of antiforgery cookie-token is not valid anymore
            // Since the tokens are generated based on the authenticated user!
            // We need a new token after authentication (The cookie can stay the same)
            antiforgeryToken = null;
        }
    }
}
