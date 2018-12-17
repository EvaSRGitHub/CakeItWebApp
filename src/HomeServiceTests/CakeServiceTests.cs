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
        public async Task UpdateCake_WithValidData_ShouldSaveUpdatedProductInDb()
        {
            //Arange
            var repo = new Repository<Product>(this.Db);

            var cakeModel1 = new CreateCakeViewModel { Name = "Chocolate Peanut Cake", CategoryId = 1, Price = 35.50m, Description = "This Chocolate and Peanut Butter Drip Cake is completely sinful.", Image = "https://res.cloudinary.com/cakeit/image/upload/ar_1:1,c_fill,g_auto,e_art:hokusai/v1544136590/Chocolate_and_Peanut_cake.jpg" };

            var cakeService = new CakeService(null, repo, this.Mapper);

            await cakeService.AddCakeToDb(cakeModel1);

            var entity = this.Db.Products.Find(1);

            this.Db.Entry(entity).State = EntityState.Detached;

            entity.Price = 45.00m;

           //Act
            var result = await cakeService.UpdateCake(this.Mapper.Map<Product, EditAndDeleteViewModel>(entity));

            var actual = await cakeService.GetCakeById(1);

            decimal price = actual.Price;

            //Assert
            Assert.Equal(45, price);
        }

        [Fact]
        public async Task UpdateCake_WithdDuplicateName_ShouldNotUpdatedProduct()
        {
            //Arange
            var repo = new Repository<Product>(this.Db);

            var cakeModel1 = new CreateCakeViewModel { Name = "Chocolate Peanut Cake", CategoryId = 1, Price = 35.50m, Description = "This Chocolate and Peanut Butter Drip Cake is completely sinful.", Image = "https://res.cloudinary.com/cakeit/image/upload/ar_1:1,c_fill,g_auto,e_art:hokusai/v1544136590/Chocolate_and_Peanut_cake.jpg" };

            var cakeModel2 = new CreateCakeViewModel { Name = "Chocolate Cake", CategoryId = 1, Price = 35.50m, Description = "This Chocolate and Peanut Butter Drip Cake is completely sinful.", Image = "https://res.cloudinary.com/cakeit/image/upload/ar_1:1,c_fill,g_auto,e_art:hokusai/v1544136591/Chocolate_cake.jpg" };

            var cakeService = new CakeService(null, repo, this.Mapper);

            await cakeService.AddCakeToDb(cakeModel1);
            await cakeService.AddCakeToDb(cakeModel2);

            var entity = this.Db.Products.Find(1);

            this.Db.Entry(entity).State = EntityState.Detached;

            var model = this.Mapper.Map<Product, EditAndDeleteViewModel>(entity);
                model.Name = "Chocolate Cake";

            //Act
            var result = await cakeService.UpdateCake(model);

            var actual = await cakeService.GetCakeById(1);

            string name = actual.Name;

            //Assert
            Assert.Equal("Chocolate Peanut Cake", name);
            Assert.Equal("Product with such name already exists.", result);
        }

        [Fact]
        public async Task UpdateCake_WithdDuplicateImageUrl_ShouldNotUpdatedProduct()
        {
            //Arange
            var repo = new Repository<Product>(this.Db);

            var cakeModel1 = new CreateCakeViewModel { Name = "Chocolate Peanut Cake", CategoryId = 1, Price = 35.50m, Description = "This Chocolate and Peanut Butter Drip Cake is completely sinful.", Image = "https://res.cloudinary.com/cakeit/image/upload/ar_1:1,c_fill,g_auto,e_art:hokusai/v1544136590/Chocolate_and_Peanut_cake.jpg" };

            var cakeModel2 = new CreateCakeViewModel { Name = "Chocolate Cake", CategoryId = 1, Price = 35.50m, Description = "This Chocolate and Peanut Butter Drip Cake is completely sinful.", Image = "https://res.cloudinary.com/cakeit/image/upload/ar_1:1,c_fill,g_auto,e_art:hokusai/v1544136591/Chocolate_cake.jpg" };

            var cakeService = new CakeService(null, repo, this.Mapper);

            await cakeService.AddCakeToDb(cakeModel1);
            await cakeService.AddCakeToDb(cakeModel2);

            var entity = this.Db.Products.Find(1);

            this.Db.Entry(entity).State = EntityState.Detached;

            var model = this.Mapper.Map<Product, EditAndDeleteViewModel>(entity);
            model.Image = "https://res.cloudinary.com/cakeit/image/upload/ar_1:1,c_fill,g_auto,e_art:hokusai/v1544136591/Chocolate_cake.jpg";

            //Act
            var result = await cakeService.UpdateCake(model);

            var actual = await cakeService.GetCakeById(1);

            string imageExpected = "https://res.cloudinary.com/cakeit/image/upload/ar_1:1,c_fill,g_auto,e_art:hokusai/v1544136590/Chocolate_and_Peanut_cake.jpg";
            string imageActual = actual.Image;

            //Assert
            Assert.Equal(imageExpected, imageActual);
            Assert.Equal("Product with such image url already exists.", result);
        }

        [Fact]
        public async Task DeleteCake_WithInvalidId_ShouldDoNoting()
        {
            //Arange
            var repo = new Repository<Product>(this.Db);

            var cakeModel1 = new CreateCakeViewModel { Name = "Chocolate Peanut Cake", CategoryId = 1, Price = 35.50m, Description = "This Chocolate and Peanut Butter Drip Cake is completely sinful.", Image = "https://res.cloudinary.com/cakeit/image/upload/ar_1:1,c_fill,g_auto,e_art:hokusai/v1544136590/Chocolate_and_Peanut_cake.jpg" };

            var cakeService = new CakeService(null, repo, this.Mapper);

            await cakeService.AddCakeToDb(cakeModel1);

            //Act
            await cakeService.DeleteCake(2);

            var actualProductsCount = this.Db.Products.Count(); ;

            var expectedProductsCount = 1;

            //Assert
            Assert.Equal(expectedProductsCount, actualProductsCount);
        }

        [Fact]
        public async Task DeleteCake_WithValidId_ShouldDeleteProduct()
        {
            //Arange
            var repo = new Repository<Product>(this.Db);

            var cakeModel1 = new CreateCakeViewModel { Name = "Chocolate Peanut Cake", CategoryId = 1, Price = 35.50m, Description = "This Chocolate and Peanut Butter Drip Cake is completely sinful.", Image = "https://res.cloudinary.com/cakeit/image/upload/ar_1:1,c_fill,g_auto,e_art:hokusai/v1544136590/Chocolate_and_Peanut_cake.jpg" };

            var cakeService = new CakeService(null, repo, this.Mapper);

            await cakeService.AddCakeToDb(cakeModel1);

            //Act
            await cakeService.DeleteCake(1);

            var actualProductsCount = this.Db.Products.Count(); ;

            var expectedProductsCount = 0;

            //Assert
            Assert.Equal(expectedProductsCount, actualProductsCount);
        }

        [Fact]
        public async Task ShowCakeDetails_WithValidId_ShouldReturnCakeDetailsViewModel()
        {
            //Arrange
            await this.SeedProducts();
            var repo = new Repository<Product>(this.Db);
            var service = new CakeService(null, repo, this.Mapper);

            //Act
            var cakeDetails = await service.ShowCakeDetails(2);

            //Assert
            Assert.NotNull(cakeDetails);
            Assert.Equal(2, cakeDetails.Id);
        }

        [Fact]
        public async Task ShowCakeDetails_WithInValidId_ShouldReturnNull()
        {
            //Arrange
            await this.SeedProducts();
            var repo = new Repository<Product>(this.Db);
            var service = new CakeService(null, repo, this.Mapper);

            //Act
            var cakeDetails = await service.ShowCakeDetails(3);

            //Assert
            Assert.Null(cakeDetails);
        }

        [Fact]
        public async Task AddRatingToCake_WithValidData_ShouldAddRatingToProduct()
        {
            //Arrange
            await this.SeedProducts();
            var repo = new Repository<Product>(this.Db);
            var service = new CakeService(null, repo, this.Mapper);

            //Act
            await service.AddRatingToCake(2, 5);
            await repo.SaveChangesAsync();
            var expectedRate = 5;
            var actualRate = repo.All().SingleOrDefault(p => p.Id == 2).Rating;

            //Assert
            Assert.Equal(expectedRate, actualRate);
        }

        [Fact]
        public async Task AddRatingToCake_WithValidData_ShoulChangeRatingVotesOfProduct()
        {
            //Arrange
            await this.SeedProducts();
            var repo = new Repository<Product>(this.Db);
            var service = new CakeService(null, repo, this.Mapper);

            //Act
            await service.AddRatingToCake(2, 5);
            await repo.SaveChangesAsync();
            var expectedRatingVote = 1;
            var actualRatingVote = repo.All().SingleOrDefault(p => p.Id == 2).RatingVotes;

            //Assert
            Assert.Equal(expectedRatingVote, actualRatingVote);
        }

        [Fact]
        public async Task AddRatingToCake_WithAddingRate_ShoulIncreaseRating()
        {
            //Arrange
            await this.SeedProducts();
            var repo = new Repository<Product>(this.Db);
            var service = new CakeService(null, repo, this.Mapper);

            //Act
            await service.AddRatingToCake(2, 5);
            await service.AddRatingToCake(2, 3);
            await repo.SaveChangesAsync();
            var expectedRate = 8;
            var actualRate = repo.All().SingleOrDefault(p => p.Id == 2).Rating;

            //Assert
            Assert.Equal(expectedRate, actualRate);
        }

        [Fact]
        public async Task AddRatingToCake_WithAddingRate_ShoulIncreaseRatingVotes()
        {
            //Arrange
            await this.SeedProducts();
            var repo = new Repository<Product>(this.Db);
            var service = new CakeService(null, repo, this.Mapper);

            //Act
            await service.AddRatingToCake(2, 5);
            await service.AddRatingToCake(2, 3);
            await repo.SaveChangesAsync();
            var expectedRatingVotes = 2;
            var actualRatingVotes = repo.All().SingleOrDefault(p => p.Id == 2).RatingVotes;

            //Assert
            Assert.Equal(expectedRatingVotes, actualRatingVotes);
        }

        [Fact]
        public async Task AddRatingToCake_WithInValidCakeId_ShoulThrow()
        {
            //Arrange
            await this.SeedProducts();
            var repo = new Repository<Product>(this.Db);
            var service = new CakeService(null, repo, this.Mapper);

            //Act

            //Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await service.AddRatingToCake(3, 5));
        }

        [Fact]
        public async Task AddRatingToCake_WithInValidRate_ShoulThrow()
        {
            //Arrange
            await this.SeedProducts();
            var repo = new Repository<Product>(this.Db);
            var service = new CakeService(null, repo, this.Mapper);

            //Act

            //Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await service.AddRatingToCake(3, -5));
        }

    }
}
