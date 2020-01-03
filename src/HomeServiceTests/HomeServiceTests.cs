using CakeItWebApp.Data;
using CakeItWebApp.Models;
using CakeItWebApp.Services.Common.Repository;
using CakeWebApp.Services.Common.CommonServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CakeItWebApp.Services.Common.Tests.HomeServiceTests
{
    public class HomeServiceTests : BaseServiceTestClass
    {
        private CakeItDbContext SetDb()
        {
            var serviceProvider = new ServiceCollection()
           .AddEntityFrameworkInMemoryDatabase()
           .BuildServiceProvider();

            var builder = new DbContextOptionsBuilder<CakeItDbContext>();
            builder.UseInMemoryDatabase($"database{Guid.NewGuid()}")
                   .UseInternalServiceProvider(serviceProvider);


            var Db = new CakeItDbContext(builder.Options);
            return Db;
        }

        [Fact]
        public async Task GetRandomCake__Home_Index_ShouldReturnRandomPrduct()
        {
            var db = this.SetDb();
            using (db)
            {
                //Arrange
                db.Products.Add(new Product { Name = "Chocolate Peanut Cake", Price = 35.50m, Description = "This Chocolate and Peanut Butter Drip Cake is completely sinful.", Image = "https://res.cloudinary.com/cakeit/image/upload/ar_1:1,c_fill,g_auto,e_art:hokusai/v1544136590/Chocolate_and_Peanut_cake.jpg", Category = new Category { Id = 1, Type = Models.Enums.CategoryType.Cake }});

                db.Products.Add(new Product { Name = "Chocolate Lovers Cake", Price = 30.00m, Description = "This Chocolate Lovers Cake is completely sinful.", Image = "https://res.cloudinary.com/cakeit/image/upload/ar_1:1,c_fill,g_auto,e_art:hokusai/v1544136590/Chocolate_and_Peanut_cake.jpg", Category = new Category { Id = 1, Type = Models.Enums.CategoryType.Cake } });

                await db.SaveChangesAsync();

                var repo = new Repository<Product>(db);
                var homeService = new HomeService(repo, this.Mapper);

                //Act
                var result = await homeService.GetRandomCake();

                //Assert
                result.ShouldNotBeNull();
            }
        }

        [Fact]
        public async Task GetRandomCake_Home_Index_WhenFirstProductIsDeletedTrue_ShouldReturnProductId2()
        {
            var db = this.SetDb();
            using (db)
            {
                //Arrange
                db.Products.Add(new Product { Name = "Chocolate Peanut Cake", Price = 35.50m, Description = "This Chocolate and Peanut Butter Drip Cake is completely sinful.", Image = "https://res.cloudinary.com/cakeit/image/upload/ar_1:1,c_fill,g_auto,e_art:hokusai/v1544136590/Chocolate_and_Peanut_cake.jpg", Category = new Category { Id = 1, Type = Models.Enums.CategoryType.Cake }, IsDeleted = true });

                db.Products.Add(new Product { Name = "Chocolate Lovers Cake", Price = 30.00m, Description = "This Chocolate Lovers Cake is completely sinful.", Image = "https://res.cloudinary.com/cakeit/image/upload/ar_1:1,c_fill,g_auto,e_art:hokusai/v1544136590/Chocolate_and_Peanut_cake.jpg", Category = new Category { Id = 1, Type = Models.Enums.CategoryType.Cake } });

                await db.SaveChangesAsync();

                var repo = new Repository<Product>(db);

                var homeService = new HomeService(repo, this.Mapper);

                var actual = repo.All().SingleOrDefault(p => p.Id == 2).Name;

                //Act
                var result = await homeService.GetRandomCake();

                //Assert
                Assert.Equal(actual, result.Name);
            }
        }

        [Fact]
        public async Task GetRandomCake__Home_Index_ShouldReturnNull()
        {
            var db = this.SetDb();
            using (db)
            {
                //Arrange
                db.Products.Add(new Product { Name = "Chocolate Peanut Cake", Price = 35.50m, Description = "This Chocolate and Peanut Butter Drip Cake is completely sinful.", Image = "https://res.cloudinary.com/cakeit/image/upload/ar_1:1,c_fill,g_auto,e_art:hokusai/v1544136590/Chocolate_and_Peanut_cake.jpg", Category = new Category { Id = 1, Type = Models.Enums.CategoryType.Cake }, IsDeleted = true });

                await db.SaveChangesAsync();

                var repo = new Repository<Product>(db);
                var homeService = new HomeService(repo, this.Mapper);

                //Act
                var result = await homeService.GetRandomCake();

                //Assert
                result.ShouldBeNull();
            }
        }
    }
}
