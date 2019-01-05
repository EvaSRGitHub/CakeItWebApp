using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using CakeWebApp.Services.Common.CommonServices;
using CakeItWebApp.Services.Common.Repository;
using CakeWebApp.Services.Common.Contracts;

namespace CakeItWebApp.Middlewares
{
    public class ClearShoppingCartMiddleware
    {
        private const string CartSessionKey = "CartId";

        private readonly RequestDelegate next;

        public ClearShoppingCartMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext context, IServiceProvider provider)
        {
            ISession session = provider.GetService<IHttpContextAccessor>().HttpContext.Session;

            string cartId = session?.GetString(CartSessionKey);

            if (cartId == null)
            {
                var service = provider.GetService<IShoppingCartService>();

                await service.ClearShoppingCartWhenUserLeft();

                await this.next(context);
            }

            await this.next(context);
        }
    }
}
