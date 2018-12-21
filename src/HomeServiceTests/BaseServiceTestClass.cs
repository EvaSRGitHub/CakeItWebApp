using AutoMapper;
using CakeItWebApp.Data;
using CakeItWebApp.Models;
using CakeItWebApp.Services.Common.Repository;
using CakeItWebApp.ViewModels;
using CakeItWebApp.ViewModels.Cakes;
using CakeItWebApp.ViewModels.CustomCake;
using CakeItWebApp.ViewModels.Orders;
using CakeItWebApp.ViewModels.Tutorials;
using CakeWebApp.Services.Common.CommonServices;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CakeItWebApp.Services.Common.Tests
{
    public abstract class BaseServiceTestClass
    {
        protected BaseServiceTestClass()
        {
            this.Mapper = new Mapper(new MapperConfiguration(
                config => {
                    config.CreateMap<Product, CakeIndexViewModel>().ReverseMap();
                    config.CreateMap<Product, CreateCakeViewModel>().ReverseMap();
                    config.CreateMap<Product, EditAndDeleteViewModel>().ReverseMap();
                    config.CreateMap<Order, OrderViewModel>().ReverseMap();
                    config.CreateMap<ShoppingCartItem, OrderedProductsViewModel>().ReverseMap();
                    config.CreateMap<OrderDetails, OrderDetailsViewModel>().ReverseMap();
                    config.CreateMap<CustomCakeImgViewModel, CustomCakeImg>().ReverseMap();
                    config.CreateMap<CustomCakeOrderViewModel, Ingredients>().ReverseMap();
                    config.CreateMap<AddTutorialViewModel, Tutorial>().ReverseMap();
                    config.CreateMap<TutorialIndexViewModel, Tutorial>().ReverseMap();
                }));

            this.Options = new DbContextOptionsBuilder<CakeItDbContext>().UseInMemoryDatabase(databaseName: "CakeItInMemory").Options;

            this.Db = new CakeItDbContext(this.Options);
        }

        protected IMapper Mapper { get; private set; }

        protected DbContextOptions<CakeItDbContext> Options { get; private set; }

        public CakeItDbContext Db { get; protected set; }

        protected async Task SeedProducts()
        {
            var repo = new Repository<Product>(this.Db);

            var cakeModel1 = new CreateCakeViewModel { Name = "Chocolate Peanut Cake", CategoryId = 1, Price = 35.50m, Description = "This Chocolate and Peanut Butter Drip Cake is completely sinful.", Image = "https://res.cloudinary.com/cakeit/image/upload/ar_1:1,c_fill,g_auto,e_art:hokusai/v1544136590/Chocolate_and_Peanut_cake.jpg" };

            var cakeModel2 = new CreateCakeViewModel { Name = "Chocolate Cake", CategoryId = 1, Price = 35.50m, Description = "This Chocolate and Peanut Butter Drip Cake is completely sinful.", Image = "https://res.cloudinary.com/cakeit/image/upload/ar_1:1,c_fill,g_auto,e_art:hokusai/v1544136590/Chocolate_and_Peanut_cake.jpg" };

            var cakeService = new CakeService(null, repo, this.Mapper);

           await cakeService.AddCakeToDb(cakeModel1);
           await cakeService.AddCakeToDb(cakeModel2);
        }
    }
}
