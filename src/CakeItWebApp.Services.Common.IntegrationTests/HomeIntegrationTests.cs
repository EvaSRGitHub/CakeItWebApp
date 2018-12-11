using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CakeItWebApp.Data;
using CakeItWebApp.Models;
using CakeItWebApp.Services.Common.Repository;
using CakeWebApp.Services.Common.CommonServices;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace CakeItWebApp.Services.Common.IntegrationTests
{
    public class HomeIntegrationTests : BaseIntegrationTest
    {
        public HomeIntegrationTests(WebApplicationFactory<Startup> server) : base(server)
        {
        }

        [Fact]
        public async Task Index_WithProducts_ShouldReturn200()
        {
            //Act
            var result = await this.Client.GetAsync("/");

            //Assert
            result.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task Index_WithNoProducts_ShouldReturn_ErrorView()
        {
           //Act
            var result = await this.Client.GetAsync("/");

            var respAsString = await result.Content.ReadAsStringAsync();

            //Assert
            Assert.Contains("<title>Error", respAsString);
        }
    }
}
