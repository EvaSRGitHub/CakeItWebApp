using CakeItWebApp.Data;
using CakeItWebApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CakeItWebApp.Middlewares
{
    public class SeedRolesMiddleware
    {
        private readonly RequestDelegate next;

        public SeedRolesMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext context, IServiceProvider provider)
        {
            var db = provider.GetService<CakeItDbContext>();

            if (!db.Roles.Any())
            {
                this.SeedRoles(provider);
            }

            if (!db.UserRoles.Any(r => r.RoleId == db.Roles.First(a => a.Name == "Admin").Id))
            {
                this.SeedAdmin(provider);
            }

            await this.next(context);
        }

        private void SeedAdmin(IServiceProvider provider)
        {
            var signInManager = provider.GetRequiredService<SignInManager<CakeItUser>>();
            var configuration = provider.GetRequiredService<IConfiguration>();

            // Seed User admin
            var email = configuration["AdminUser:Email"];

            var pass = configuration["AdminUser:Password"];

            var user = new CakeItUser
            {
                UserName = email,
                Email = email
            };

            var result = signInManager.UserManager.CreateAsync(user, pass).Result;

            var role = signInManager.UserManager.AddToRoleAsync(user, "Admin").Result;
        }

        private void SeedRoles(IServiceProvider provider)
        {
            var roleManager = provider.GetRequiredService<RoleManager<IdentityRole>>();

            Task<IdentityResult> roleResult;

            string[] roleNames = { "Admin", "User" };

            foreach (var role in roleNames)
            {
                Task<bool> hasAdminRole = roleManager.RoleExistsAsync(role);
                hasAdminRole.Wait();

                if (!hasAdminRole.Result)
                {
                    roleResult = roleManager.CreateAsync(new IdentityRole(role));
                    roleResult.Wait();
                }
            }
        }
    }
}
