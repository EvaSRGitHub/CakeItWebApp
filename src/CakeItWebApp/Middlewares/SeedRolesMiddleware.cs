﻿using CakeItWebApp.Data;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using CakeItWebApp.Models;

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
            var configuration = provider.GetRequiredService<Startup>();

            // Seed User admin
            var email = configuration.Configuration.GetSection("AdminUser").GetChildren().ToList()[0].Value;
            var pass = configuration.Configuration.GetSection("AdminUser").GetChildren().ToList()[1].Value;

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
            var userManager = provider.GetRequiredService<UserManager<CakeItUser>>();
            var signInManager = provider.GetRequiredService<SignInManager<CakeItUser>>();
            var configuration = provider.GetRequiredService<Startup>();

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