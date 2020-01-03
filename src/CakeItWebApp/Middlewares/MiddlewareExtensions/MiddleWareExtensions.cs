using Microsoft.AspNetCore.Builder;

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

        public static IApplicationBuilder UseCreateCategoryMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CreateCategoryMiddleware>();
        }

        public static IApplicationBuilder UseSeedCakesMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SeedCakesMiddleware>();
        }

        public static IApplicationBuilder UseSeedCustomCakesImgsMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SeedCustomCakesImgsMiddleware>();
        }
    }
}
