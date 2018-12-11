using CakeItWebApp.Data;
using CakeItWebApp.Models;
using CakeItWebApp.Services.Common.Repository;
using CakeItWebApp.ViewModels;
using CakeItWebApp.ViewModels.Cakes;
using CakeWebApp.Services.Common.CommonServices;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CakeItWebApp.Services.Common.Tests
{
    public class CakeServiceTests : BaseServiceTestClass
    {
        [Fact]
        public async Task AddCakeToDb_WithValidCake_ShouldReturnTrue()
        {
            //Arrange
            var repo = new Repository<Product>(this.Db);

            var cakeModel = new CreateCakeViewModel { Name = "Chocolate Peanut Cake", CategoryId = 1, Price = 35.50m, Description = "This Chocolate and Peanut Butter Drip Cake is completely sinful.", Image = "https://res.cloudinary.com/cakeit/image/upload/ar_1:1,c_fill,g_auto,e_art:hokusai/v1544136590/Chocolate_and_Peanut_cake.jpg" };

            var cakeService = new CakeService(null, repo, this.Mapper);
            
            //Act
            var result = await cakeService.AddCakeToDb(cakeModel);

            //Assert
            Assert.Equal("true", result);
        }

        [Fact]
        public async Task AddCakeToDb_WithDuplicateCake_ShouldNotReturnTrue()
        {
            //Arrange
            var repo = new Repository<Product>(this.Db);

            var cakeModel1 = new CreateCakeViewModel { Name = "Chocolate Peanut Cake", CategoryId = 1, Price = 35.50m, Description = "This Chocolate and Peanut Butter Drip Cake is completely sinful.", Image = "https://res.cloudinary.com/cakeit/image/upload/ar_1:1,c_fill,g_auto,e_art:hokusai/v1544136590/Chocolate_and_Peanut_cake.jpg" };

            var cakeModel2 = new CreateCakeViewModel { Name = "Chocolate Peanut Cake", CategoryId = 1, Price = 35.50m, Description = "This Chocolate and Peanut Butter Drip Cake is completely sinful.", Image = "https://res.cloudinary.com/cakeit/image/upload/ar_1:1,c_fill,g_auto,e_art:hokusai/v1544136590/Chocolate_and_Peanut_cake.jpg" };

            var cakeService = new CakeService(null, repo, this.Mapper);

            //Act
            var result1 = await cakeService.AddCakeToDb(cakeModel1);
            var result2 = await cakeService.AddCakeToDb(cakeModel2);

            //Assert
            Assert.NotEqual("true", result2);
        }

        [Fact]
        public void GetAllCakes_WithOutProductsInDb_ShouldReturnProduct()
        {
            //Arange
            var repo = new Repository<Product>(this.Db);

            var cakeService = new CakeService(null, repo, this.Mapper);
            
            //Act
            var result = this.Mapper.ProjectTo<CakeIndexViewModel>(repo.All()).ToList();

            //Assert
            result.ShouldBeEmpty();
        }

        [Fact]
        public async Task GetAllCakes_WithProductsInDb_ShouldReturnProduct()
        {
            //Arange
            var repo = new Repository<Product>(this.Db);

            var cakeModel1 = new CreateCakeViewModel { Name = "Chocolate Peanut Cake", CategoryId = 1, Price = 35.50m, Description = "This Chocolate and Peanut Butter Drip Cake is completely sinful.", Image = "https://res.cloudinary.com/cakeit/image/upload/ar_1:1,c_fill,g_auto,e_art:hokusai/v1544136590/Chocolate_and_Peanut_cake.jpg" };

            var cakeModel2 = new CreateCakeViewModel { Name = "Chocolate Cake", CategoryId = 1, Price = 35.50m, Description = "This Chocolate and Peanut Butter Drip Cake is completely sinful.", Image = "https://res.cloudinary.com/cakeit/image/upload/ar_1:1,c_fill,g_auto,e_art:hokusai/v1544136590/Chocolate_and_Peanut_cake.jpg" };

            var cakeService = new CakeService(null, repo, this.Mapper);

            var result1 = await cakeService.AddCakeToDb(cakeModel1);
            var result2 = await cakeService.AddCakeToDb(cakeModel2);

            //Act

            var result = cakeService.GetAllCakes();

            //Assert
            result.ShouldNotBeEmpty();
        }

        [Fact]
        public async Task GetCakesById_WithValidId_ShouldReturnProduct()
        {
            //Arange
            var repo = new Repository<Product>(this.Db);

            var cakeModel1 = new CreateCakeViewModel { Name = "Chocolate Peanut Cake", CategoryId = 1, Price = 35.50m, Description = "This Chocolate and Peanut Butter Drip Cake is completely sinful.", Image = "https://res.cloudinary.com/cakeit/image/upload/ar_1:1,c_fill,g_auto,e_art:hokusai/v1544136590/Chocolate_and_Peanut_cake.jpg" };

            var cakeModel2 = new CreateCakeViewModel { Name = "Chocolate Cake", CategoryId = 1, Price = 35.50m, Description = "This Chocolate and Peanut Butter Drip Cake is completely sinful.", Image = "https://res.cloudinary.com/cakeit/image/upload/ar_1:1,c_fill,g_auto,e_art:hokusai/v1544136590/Chocolate_and_Peanut_cake.jpg" };

            var cakeService = new CakeService(null, repo, this.Mapper);

            var result1 = await cakeService.AddCakeToDb(cakeModel1);
            var result2 = await cakeService.AddCakeToDb(cakeModel2);

            //Act
            var result = await cakeService.GetCakeById(1);
            var productId = this.Db.Products.SingleOrDefault(p => p.Name == result.Name).Id;

            //Assert
            Assert.Equal(1, productId);
            result.ShouldBeOfType<EditAndDeleteViewModel>();
        }

        [Fact]
        public async Task GetCakesById_WithInValidId_ShouldReturnNull()
        {
            //Arange
            var repo = new Repository<Product>(this.Db);

            var cakeModel = new CreateCakeViewModel { Name = "Chocolate Peanut Cake", CategoryId = 1, Price = 35.50m, Description = "This Chocolate and Peanut Butter Drip Cake is completely sinful.", Image = "https://res.cloudinary.com/cakeit/image/upload/ar_1:1,c_fill,g_auto,e_art:hokusai/v1544136590/Chocolate_and_Peanut_cake.jpg" };

            var cakeService = new CakeService(null, repo, this.Mapper);

            var productToDb = await cakeService.AddCakeToDb(cakeModel);

            //Act
            var result = await (cakeService.GetCakeById(2));

            //Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task UpdateProduct_WithValidData_ShouldSaveUpdatedProductInDb()
        {
            //Arange
            var repo = new Repository<Product>(this.Db);

            var cakeModel1 = new CreateCakeViewModel { Name = "Chocolate Peanut Cake", CategoryId = 1, Price = 35.50m, Description = "This Chocolate and Peanut Butter Drip Cake is completely sinful.", Image = "https://res.cloudinary.com/cakeit/image/upload/ar_1:1,c_fill,g_auto,e_art:hokusai/v1544136590/Chocolate_and_Peanut_cake.jpg" };

            var cakeService = new CakeService(null, repo, this.Mapper);

             await cakeService.AddCakeToDb(cakeModel1);

            //var entity = this.Db.Products.Find(1);

            // this.Db.Entry(entity).State = EntityState.Detached;
            var entity = this.Db.Products.Find(1);
            this.Db.Entry(entity).State = EntityState.Detached;

            entity.Price = 45.00m;

            var result = await cakeService.UpdateProduct(this.Mapper.Map <Product, EditAndDeleteViewModel>(entity));

            //this.Db.Products.Update(entity);
            //this.Db.SaveChanges();

            //this.Db.Products.Update(this.Mapper.Map<EditAndDeleteViewModel, Product>(productById));
            //this.Db.SaveChanges();
            //var product = this.Mapper.Map<EditAndDeleteViewModel, Product>(productById);

            //this.Db.Entry(product).State = EntityState.Detached;

            //product.Price = 45.00m;

            //var productToEdit = this.Mapper.Map<Product, EditAndDeleteViewModel>(product);

            ////Act
            //var result =  cakeService.UpdateProduct(productToEdit).GetAwaiter().GetResult();

            var actual = (await cakeService.GetCakeById(1));
            decimal price = actual.Price;
            //Assert
            Assert.Equal(45, price);
        }
    }
}
