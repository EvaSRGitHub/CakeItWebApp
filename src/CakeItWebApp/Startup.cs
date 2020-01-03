using AutoMapper;
using CakeItWebApp.Data;
using CakeItWebApp.Middlewares.MiddlewareExtensions;
using CakeItWebApp.Models;
using CakeItWebApp.Services.Common.Repository;
using CakeItWebApp.Services.Messaging;
using CakeWebApp.Services.Common.Cart;
using CakeWebApp.Services.Common.CommonServices;
using CakeWebApp.Services.Common.Contracts;
using CakeWebApp.Services.Common.Sanitizer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Globalization;

namespace CakeItWebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => false;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDbContext<CakeItDbContext>(options =>
                    options.UseSqlServer(
                        this.Configuration.GetConnectionString("DefaultConnection")).UseLazyLoadingProxies());

            services.AddIdentity<CakeItUser, IdentityRole>(options => {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 3;
                options.Password.RequiredUniqueChars = 1;

                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = false;
            })
                .AddEntityFrameworkStores<CakeItDbContext>()
                .AddDefaultTokenProviders();

            services.AddMvc(options => {
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Identity/Account/Login";
                options.AccessDeniedPath = "/Identity/Account/AccessDenied";
            });

            services.AddAutoMapper();

            services.AddLogging();

            services.AddSingleton(this.Configuration);

            services.AddScoped<ICustomEmilSender, SendGridEmailSender>();

            services.AddSingleton<IErrorService, ErrorService>();

            //services.AddAuthentication().AddFacebook(facebookOptions =>
            //{
            //    facebookOptions.AppId = Configuration["Authentication:Facebook:AppId"];
            //    facebookOptions.AppSecret = Configuration["Authentication:Facebook:AppSecret"];
            //});

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(60);
                options.Cookie.HttpOnly = true;
            });

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            services.AddScoped<IHomeService, HomeService>();

            services.AddScoped<ICakeService, CakeService>();

            services.AddScoped<IOrderService, OrderService>();

            services.AddScoped<IOrderDetailsService, OrderDetailsService>();

            services.AddScoped<ICustomCakeService, CustomCakeService>();

            services.AddScoped<ITutorialService, TutorialService>();

            services.AddScoped<ITagService, TagService>();

            services.AddScoped<IForumService, ForumService>();

            services.AddScoped<ISanitizer, HtmlSanitizerAdapter>();

            services.AddScoped<IBookService, BookService>();

            services.AddScoped<ICartService, CartService>();

            services.AddScoped<ICartManager, CartManager>();

            services.AddSingleton<ICartSessionWrapper, CartSessionWrapper>();

            services.AddScoped<IEmailService, EmailService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            var supportedCultures = new[]
            {
                new CultureInfo("en-US"),
                new CultureInfo("bg-BG"),
            };

            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("en-US"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            });

            app.UseSeedRolesMiddleware();

            app.UseCreateCategoryMiddleware();

            app.UseSeedCakesMiddleware();

            app.UseSeedCustomCakesImgsMiddleware();

            app.UseError404Middleware();

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseCookiePolicy();

            app.UseSession();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                     name: "areaRoute",
                     template: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });


            ////Only for testing purposes.To be deleted.
            //provider.GetRequiredService<ICustomEmilSender>().SendEmailAsync(new SendEmailDetails
            //{
            //    FromEmail = this.Configuration["SenderEmailDetails:FromEmail"],
            //    FromName = this.Configuration["SenderEmailDetails:FromName"],
            //    ToEmail = "evarakova79@gmail.com",
            //    ToName = "My Lady",
            //    Subject = "Test email",
            //    Content = "This is my second test mail",
            //    IsHtml = false
            //});
        }
    }
}
