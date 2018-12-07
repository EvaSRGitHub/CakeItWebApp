using AutoMapper;
using CakeItWebApp.Data;
using CakeItWebApp.Models;
using CakeItWebApp.Services.Common.Repository;
using CakeItWebApp.ViewModels.Home;
using CakeWebApp.Services.Common.CommonServices;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CakeItWebApp.Services.Common.Tests.HomeServiceTests
{
    public class HomeServiceTests:BaseServiceTestClass
    {

        [Fact]
        public async Task CakeById_Home_Index_ShouldReturnCake()
        { 
            var db = new CakeItDbContext(this.Options);

            using (db)
            {
                db.Products.Add(new Product { Name = "Chocolate Peanut Cake", Price = 35.50m, Description = "This Chocolate and Peanut Butter Drip Cake is completely sinful.", Image = "https://res.cloudinary.com/cakeit/image/upload/ar_1:1,c_fill,g_auto,e_art:hokusai/v1544136590/Chocolate_and_Peanut_cake.jpg" });

                await db.SaveChangesAsync();

                var repo = new Repository<Product>(db);
                var homeService = new HomeService(repo, Mapper);

                var result = await homeService.GetCakeById(1);

                Assert.Equal(db.Products.First().Name, result.Name);
            }
        }

        [Fact]
        public async Task CakeById_EmptyProduct_Home_Index_ShouldReturnErrorPage()
        {
            var db = new CakeItDbContext(this.Options);

            using (db)
            {
                var repo = new Repository<Product>(db);

                var homeService = new HomeService(repo, Mapper);

               var result = homeService.GetCakeProductsCount();

                Assert.Equal(0, result);
            }
        }
    }
}
