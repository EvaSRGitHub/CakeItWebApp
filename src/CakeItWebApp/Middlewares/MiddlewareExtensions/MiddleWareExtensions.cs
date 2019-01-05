using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CakeItWebApp.Middlewares.MiddlewareExtensions
{
    public static class MiddleWareExtensions
    {
        public static IApplicationBuilder UseSeedRolesMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SeedRolesMiddleware>();
        }

        public static IApplicationBuilder UseError404Middleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<Error404Middleware>();
        }

        public static IApplicationBuilder UseClearShoppingCartMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ClearShoppingCartMiddleware>();
        }
    }
}
