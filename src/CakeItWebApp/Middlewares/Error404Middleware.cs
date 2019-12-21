using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace CakeItWebApp.Middlewares
{
    public class Error404Middleware
    {
        private readonly RequestDelegate next;

        public Error404Middleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext context, IServiceProvider provider)
        {
            await this.next(context);

            if (context.Response.StatusCode == 404 && !context.Response.HasStarted)
            {
                //Re-execute the request so the user gets the error page
                string originalPath = context.Request.Path.Value;
                context.Items["originalPath"] = originalPath;
                context.Request.Path = "/Home/Error404";

                await this.next(context);
            }
        }
    }
}
