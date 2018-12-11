using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace CakeItWebApp.Services.Common.IntegrationTests
{
    public class CakeIntegrationTests : BaseIntegrationTest
    {
        public CakeIntegrationTests(WebApplicationFactory<Startup> server) : base(server)
        {
        }

        [Fact]
        public async Task AddCakeToDb_WithNotAuthenicatedUser_ShouldReturn404()
        {
            //Act
            var result = await this.Client.GetAsync("/Cakes/Create");

            var httpStatusCode = result.StatusCode.ToString();

            //Assert
            Assert.Equal("NotFound", httpStatusCode);
        }

        [Fact]
        public async Task AddCakeToDb_WithAuthenicatedUser_ShouldReturn200()
        {
            // Arrange
            this.Client = this.Server.CreateClient(new WebApplicationFactoryClientOptions()
            { AllowAutoRedirect = false });

            await EnsureAdminAuthenticationCookie();

            // Act
            var response = await this.Client.GetAsync("/Cakes/Create");

            // Assert
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task AddCakeToDb_WithInvalidData_ShouldReturnErrorView()
        {
            // Arrange
            this.Client = this.Server.CreateClient(new WebApplicationFactoryClientOptions()
            { AllowAutoRedirect = false });

            await EnsureAdminAuthenticationCookie();

            //it fails here and i couldn't figure out why!!!!
            var formData = await EnsureAntiforgeryTokenForm(new Dictionary<string, string>
            {
                { "Name", "Chocolate Lovers Cake" },
                { "CategoryId", "mock abstract" },
                { "Image", "mock contents" },
                {"Price","50.00" },
                {"Description", "Test description" }
            });

            // Act
            var response = await this.Client.PostAsync("/Cakes/Create", new FormUrlEncodedContent(formData));

            var respAsString = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Contains("<title>Error", respAsString);
        }
    }
}
