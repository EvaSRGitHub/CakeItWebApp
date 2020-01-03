using CakeItWebApp.Data;
using CakeItWebApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CakeItWebApp.Middlewares
{
    public class SeedCakesMiddleware
    {
        private readonly RequestDelegate next;

        public SeedCakesMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext context, IServiceProvider provider)
        {
            var db = provider.GetService<CakeItDbContext>();
            var logger = provider.GetService<ILogger<CakeItDbContext>>();

            if (!db.Products.Any())
            {
                await this.SeedCake(db, logger);
            }

            await this.next(context);
        }

        private async Task SeedCake(CakeItDbContext db, ILogger<CakeItDbContext> logger)
        {
            var product1 = new Product { CategoryId = 1, Name = "Choco Mint", Price = 23.20m, Description = "A chocolate sponge layered with a mint flavoured cream. Masked with icing cream, chocolate glaze drip and a range of yummy choc mint goodies on top.", Image = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRNPFKz7AZZjPSwn-e7_QwcGdDUVRI1z_fG_gQiSPI4u1wBQShmhA&s" };

            var product2 = new Product { CategoryId = 1, Name = "Cherry Blossom", Price = 55.00m, Description = "Chocolate sponge with matcha green tea buttercream. Decorated with 'cherry blossom' pink sea-salted candied popcorn and pink macarons.", Image = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRcSkgiYbXZlj8AmkDwlbE0BkCMsxKR9NlEHcbVKOL3D5WXvQQx6w&s" };

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
