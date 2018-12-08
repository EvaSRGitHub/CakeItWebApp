using CakeItWebApp.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace CakeItWebApp.Services.Common.IntegrationTests
{
    public abstract class BaseIntegrationTest : IClassFixture<WebApplicationFactory<Startup>>
    {
        protected BaseIntegrationTest(WebApplicationFactory<Startup> server)
        {
            this.Server = server;
        }

        protected WebApplicationFactory<Startup> Server { get; private set; }
    }
}
