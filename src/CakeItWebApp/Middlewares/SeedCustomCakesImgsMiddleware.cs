using Microsoft.AspNetCore.Http;
using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Threading.Tasks;
using CakeItWebApp.Data;
using Microsoft.Extensions.Logging;
using CakeItWebApp.Models;

namespace CakeItWebApp.Middlewares
{
    public class SeedCustomCakesImgsMiddleware
    {
        private readonly RequestDelegate next;

        public SeedCustomCakesImgsMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext context, IServiceProvider provider)
        {
            var db = provider.GetService<CakeItDbContext>();
            var logger = provider.GetService<ILogger<CakeItDbContext>>();

            if (!db.CustomCakesImg.Any())
            {
                await this.SeedCustomCakesImages(db, logger);
            }

            await this.next(context);
        }

        private async Task SeedCustomCakesImages(CakeItDbContext db, ILogger<CakeItDbContext> logger)
        {
            var product1 = new CustomCakeImg {Side = "White_Chocolate_Cigarettes", Top = "Habana", Img = "https://res.cloudinary.com/cakeit/image/upload/v1545083551/Top_Habana_WhiteCigarettes.png", Name = "Habana" + " " + "White Cigarettes" };

            var product2 = new CustomCakeImg { Side = "White_Chocolate_Cigarettes", Top = "Glaszed_Berries", Img = "https://res.cloudinary.com/cakeit/image/upload/v1545225696/Top_GlazedBerries_WhiteCigarettes.png", Name = "Glaszed Berries" + " " + "White Cigarettes" };

            try
            {
                await db.AddRangeAsync(new object[] { product1, product2 });
                await db.SaveChangesAsync();

            }
            catch (Exception e)
            {
                logger.LogDebug(e.Message);
            }
        }
    }
}
