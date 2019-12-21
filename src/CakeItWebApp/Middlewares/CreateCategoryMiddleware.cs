using CakeItWebApp.Data;
using CakeItWebApp.Models;
using CakeItWebApp.Models.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CakeItWebApp.Middlewares
{
    public class CreateCategoryMiddleware
    {

        private readonly RequestDelegate next;

        public CreateCategoryMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext context, IServiceProvider provider)
        {
            var db = provider.GetService<CakeItDbContext>();

            if (!db.Categories.Any())
            {
                await this.SeedCategories(db, provider);
            }

            await this.next(context);
        }

        private async Task SeedCategories(CakeItDbContext db, IServiceProvider provider)
        {
            var logger = provider.GetService<ILogger<CakeItDbContext>>();

            var cake = new Category();
            cake.Type = (CategoryType)Enum.Parse(typeof(CategoryType), "Cake");
            await db.AddAsync(cake);

            var customCake = new Category();
            customCake.Type = (CategoryType)Enum.Parse(typeof(CategoryType), "CustomCake");
            await db.AddAsync(customCake);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
            }
        }
    }
}
