using AutoMapper;
using CakeItWebApp.Data;
using CakeItWebApp.Models;
using CakeItWebApp.Services.Common.Repository;
using CakeItWebApp.ViewModels;
using CakeWebApp.Services.Common.CommonServices;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Shouldly;

namespace CakeItWebApp.Services.Common.Tests.HomeServiceTests
{
    public class HomeServiceTests : BaseServiceTestClass
    {
        [Fact]
        public async Task GetRandomCake__Home_Index_ShouldReturnRandomPrduct()
        {
            using (var db = new CakeItDbContext(new DbContextOptionsBuilder<CakeItDbContext>().UseInMemoryDatabase(databaseName: "CakeItInMemory").Options))
            {
                //Arrange
                db.Products.Add(new Product { Name = "Chocolate Peanut Cake", Price = 35.50m, Description = "This Chocolate and Peanut Butter Drip Cake is completely sinful.", Image = "https://res.cloudinary.com/cakeit/image/upload/ar_1:1,c_fill,g_auto,e_art:hokusai/v1544136590/Chocolate_and_Peanut_cake.jpg" });

                db.Products.Add(new Product { Name = "Chocolate Lovers Cake", Price = 30.00m, Description = "This Chocolate Lovers Cake is completely sinful.", Image = "https://res.cloudinary.com/cakeit/image/upload/ar_1:1,c_fill,g_auto,e_art:hokusai/v1544136590/Chocolate_and_Peanut_cake.jpg" });

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
        public void GetProductCount_Home_Index_WhenNoProducts_ShouldReturn0()
        {
            using (var db = new CakeItDbContext(new DbContextOptionsBuilder<CakeItDbContext>().UseInMemoryDatabase(databaseName: "CakeItInMemory").Options))
            {
                //Arrange
                var repo = new Repository<Product>(this.Db);

                var homeService = new HomeService(repo, null);

                //Act
                var result = homeService.GetCakeProductsCount();

                //Assert
                Assert.Equal(0, result);
            }
        }

        [Fact]
        public async Task GetRandomCake_Home_Index_WhenFirstProductIsDeletedTrue_ShouldReturnProductId2()
        {
            using (var db = new CakeItDbContext(new DbContextOptionsBuilder<CakeItDbContext>().UseInMemoryDatabase(databaseName: "CakeItInMemory").Options))
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
    }
}
