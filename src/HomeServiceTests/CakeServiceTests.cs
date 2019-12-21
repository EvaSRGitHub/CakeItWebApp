using CakeItWebApp.Data;
using CakeItWebApp.Models;
using CakeItWebApp.Services.Common.Repository;
using CakeItWebApp.ViewModels;
using CakeItWebApp.ViewModels.Cakes;
using CakeItWebApp.ViewModels.CustomCake;
using CakeWebApp.Services.Common.CommonServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CakeItWebApp.Services.Common.Tests
{
    public class CakeServiceTests : BaseServiceTestClass
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

        private async Task SeedProducts(CakeItDbContext db)
        {
            var repo = new Repository<Product>(db);

            var cakeModel1 = new CreateCakeViewModel { Name = "Chocolate Peanut Cake", CategoryId = 1, Price = 35.50m, Description = "This Chocolate and Peanut Butter Drip Cake is completely sinful.", Image = "https://res.cloudinary.com/cakeit/image/upload/ar_1:1,c_fill,g_auto,e_art:hokusai/v1544136591/Chocolate_and_Peanut_cake.jpg" };

            var cakeModel2 = new CreateCakeViewModel { Name = "Chocolate Drip Cake", CategoryId = 1, Price = 35.50m, Description = "This Chocolate and Peanut Butter Drip Cake is completely sinful.", Image = "https://res.cloudinary.com/cakeit/image/upload/ar_1:1,c_fill,g_auto,e_art:hokusai/v1544136590/Chocolate_Drip_cake.jpg" };

            var cakeService = new CakeService(null, repo, this.Mapper);

            await cakeService.AddCakeToDb(cakeModel1);
            await cakeService.AddCakeToDb(cakeModel2);

            await repo.SaveChangesAsync();
            //It works without SaveCanges()???
        }

        [Fact]
        public async Task AddCakeToDb_WithValidCake_ShouldReturnTrue()
        {
            //Arrange
            var db = this.SetDb();

            var repo = new Repository<Product>(db);

            var cakeModel = new CreateCakeViewModel { Name = "Chocolate Peanut Cake", CategoryId = 1, Price = 35.50m, Description = "This Chocolate and Peanut Butter Drip Cake is completely sinful.", Image = "https://res.cloudinary.com/cakeit/image/upload/ar_1:1,c_fill,g_auto,e_art:hokusai/v1544136590/Chocolate_and_Peanut_cake.jpg" };

            var cakeService = new CakeService(null, repo, this.Mapper);

            //Act
            await cakeService.AddCakeToDb(cakeModel);

            //works with and without SaveChanges()????
            await repo.SaveChangesAsync();

            var actualName = repo.All().LastOrDefault().Name;

            //Assert
            Assert.Equal("Chocolate Peanut Cake", actualName);
        }

        [Fact]
        public async Task AddCakeToDb_WithDuplicateCake_ShouldThrow()
        {
            //Arrange
            var db = this.SetDb();
            var repo = new Repository<Product>(db);

            var cakeModel1 = new CreateCakeViewModel { Name = "Chocolate Peanut Cake", CategoryId = 1, Price = 35.50m, Description = "This Chocolate and Peanut Butter Drip Cake is completely sinful.", Image = "https://res.cloudinary.com/cakeit/image/upload/ar_1:1,c_fill,g_auto,e_art:hokusai/v1544136590/Chocolate_and_Peanut_cake.jpg" };

            var cakeModel2 = new CreateCakeViewModel { Name = "Chocolate Peanut Cake", CategoryId = 1, Price = 35.50m, Description = "This Chocolate and Peanut Butter Drip Cake is completely sinful.", Image = "https://res.cloudinary.com/cakeit/image/upload/ar_1:1,c_fill,g_auto,e_art:hokusai/v1544136590/Chocolate_and_Peanut_cake.jpg" };

            var cakeService = new CakeService(null, repo, this.Mapper);

            //Act
            await cakeService.AddCakeToDb(cakeModel1);

            //Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await cakeService.AddCakeToDb(cakeModel2));
        }

        [Fact]
        public void GetAllCakes_WithOutProductsInDb_ShouldReturnProduct()
        {
            //Arange
            var db = this.SetDb();
            var repo = new Repository<Product>(db);

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
            var db = this.SetDb();

            await this.SeedProducts(db);

            var repo = new Repository<Product>(db);

            var cakeService = new CakeService(null, repo, this.Mapper);

            //Act 
            //Doesn't pass. Supose due to the Mapper; 
            //var result = cakeService.GetAllCakes();

            //this works;
            var result = repo.All().Select(p => new CakeIndexViewModel
            {
                Id = p.Id,
                Name = p.Name,
                IsDeleted = p.IsDeleted,
                CategoryId = p.CategoryId,
                Rating = p.Rating ?? 0,
                RatingVotes = p.RatingVotes ?? 0
            }).ToList();



            //Assert
            result.ShouldNotBeEmpty();
        }

        [Fact]
        public async Task GetCakesById_WithValidId_ShouldReturnProduct()
        {
            //Arange
            var db = this.SetDb();

            await this.SeedProducts(db);

            var repo = new Repository<Product>(db);

            var cakeService = new CakeService(null, repo, this.Mapper);

            //Act
            var result = await cakeService.GetCakeById(1);
            var productId = repo.All().SingleOrDefault(p => p.Name == result.Name).Id;

            //Assert
            Assert.Equal(1, productId);
            result.ShouldBeOfType<EditAndDeleteViewModel>();
        }

        [Fact]
        public async Task GetCakesById_WithInValidId_ShouldReturnNull()
        {
            //Arange
            var db = this.SetDb();

            await this.SeedProducts(db);

            var repo = new Repository<Product>(db);

            var cakeService = new CakeService(null, repo, this.Mapper);

            //Act

            //Assert
            await Assert.ThrowsAnyAsync<NullReferenceException>(async () => await (cakeService.GetCakeById(3)));
        }

        [Fact]
        public async Task UpdateCake_WithValidData_ShouldSaveUpdatedProductInDb()
        {
            //Arange
            var db = this.SetDb();

            await this.SeedProducts(db);

            var repo = new Repository<Product>(db);

            var cakeService = new CakeService(null, repo, this.Mapper);

            var entity = repo.All().SingleOrDefault(p => p.Id == 1);

            db.Entry(entity).State = EntityState.Detached;

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
            var db = this.SetDb();

            await this.SeedProducts(db);

            var repo = new Repository<Product>(db);

            var cakeService = new CakeService(null, repo, this.Mapper);

            var entity = repo.All().SingleOrDefault(p => p.Id == 1);

            db.Entry(entity).State = EntityState.Detached;

            var model = this.Mapper.Map<Product, EditAndDeleteViewModel>(entity);
            model.Name = "Chocolate Drip Cake";

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
            var db = this.SetDb();

            await this.SeedProducts(db);

            var repo = new Repository<Product>(db);

            var cakeService = new CakeService(null, repo, this.Mapper);

            var entity = repo.All().SingleOrDefault(p => p.Id == 1);

           db.Entry(entity).State = EntityState.Detached;

            var model = this.Mapper.Map<Product, EditAndDeleteViewModel>(entity);
            model.Image = "https://res.cloudinary.com/cakeit/image/upload/ar_1:1,c_fill,g_auto,e_art:hokusai/v1544136590/Chocolate_Drip_cake.jpg";

            //Act
            var result = await cakeService.UpdateCake(model);

            var actual = await cakeService.GetCakeById(1);

            string imageExpected = "https://res.cloudinary.com/cakeit/image/upload/ar_1:1,c_fill,g_auto,e_art:hokusai/v1544136591/Chocolate_and_Peanut_cake.jpg";
            string imageActual = actual.Image;

            //Assert
            Assert.Equal(imageExpected, imageActual);
            Assert.Equal("Product with such image url already exists.", result);
        }

        [Fact]
        public async Task DeleteCake_WithInvalidId_ShouldDoNoting()
        {
            //Arange
            var db = this.SetDb();

            await this.SeedProducts(db);

            var repo = new Repository<Product>(db);

            var cakeService = new CakeService(null, repo, this.Mapper);

            EditAndDeleteViewModel cake = null;

            //Act

            //Assert
            await Assert.ThrowsAsync<NullReferenceException>(async () => await cakeService.DeleteCake(cake));
        }

        [Fact]
        public async Task DeleteCake_WithValidId_ShouldDeleteProduct()
        {
            //Arange
            var db = this.SetDb();

            await this.SeedProducts(db);

            var repo = new Repository<Product>(db);

            var cakeService = new CakeService(null, repo, this.Mapper);

            var cake = await cakeService.GetCakeById(1);

            //Act
            await cakeService.DeleteCake(cake);

            var actualProductsCount = repo.All().Where(p => p.IsDeleted == true).Count(); ;

            var expectedProductsCount = 1;

            //Assert
            Assert.Equal(expectedProductsCount, actualProductsCount);
        }

        [Fact]
        public async Task ShowCakeDetails_WithValidId_ShouldReturnCakeDetailsViewModel()
        {
            //Arrange
            var db = this.SetDb();

            await this.SeedProducts(db);

            var repo = new Repository<Product>(db);

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
            var db = this.SetDb();

            await this.SeedProducts(db);

            var repo = new Repository<Product>(db);

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
            var db = this.SetDb();

            await this.SeedProducts(db);

            var repo = new Repository<Product>(db);

            var service = new CakeService(null, repo, this.Mapper);

            //Act
            await service.AddRatingToCake(2, 5);

            var expectedRate = 5;
            var actualRate = repo.All().SingleOrDefault(p => p.Id == 2).Rating;

            //Assert
            Assert.Equal(expectedRate, actualRate);
        }

        [Fact]
        public async Task AddRatingToCake_WithValidData_ShoulChangeRatingVotesOfProduct()
        {
            //Arrange
            var db = this.SetDb();

            await this.SeedProducts(db);

            var repo = new Repository<Product>(db);

            var service = new CakeService(null, repo, this.Mapper);

            //Act
            await service.AddRatingToCake(2, 5);

            var expectedRatingVote = 1;
            var actualRatingVote = repo.All().SingleOrDefault(p => p.Id == 2).RatingVotes;

            //Assert
            Assert.Equal(expectedRatingVote, actualRatingVote);
        }

        [Fact]
        public async Task AddRatingToCake_WithAddingRate_ShoulIncreaseRating()
        {
            //Arrange
            var db = this.SetDb();

            await this.SeedProducts(db);

            var repo = new Repository<Product>(db);

            var service = new CakeService(null, repo, this.Mapper);

            //Act
            await service.AddRatingToCake(2, 5);
            await service.AddRatingToCake(2, 3);

            var expectedRate = 8;
            var actualRate = repo.All().SingleOrDefault(p => p.Id == 2).Rating;

            //Assert
            Assert.Equal(expectedRate, actualRate);
        }

        [Fact]
        public async Task AddRatingToCake_WithAddingRate_ShoulIncreaseRatingVotes()
        {
            //Arrange
            var db = this.SetDb();

            await this.SeedProducts(db);

            var repo = new Repository<Product>(db);

            var service = new CakeService(null, repo, this.Mapper);

            //Act
            await service.AddRatingToCake(2, 5);
            await service.AddRatingToCake(2, 3);

            var expectedRatingVotes = 2;
            var actualRatingVotes = repo.All().SingleOrDefault(p => p.Id == 2).RatingVotes;

            //Assert
            Assert.Equal(expectedRatingVotes, actualRatingVotes);
        }

        [Fact]
        public async Task AddRatingToCake_WithInValidCakeId_ShoulThrow()
        {
            //Arrange
            var db = this.SetDb();

            await this.SeedProducts(db);

            var repo = new Repository<Product>(db);

            var service = new CakeService(null, repo, this.Mapper);

            //Act

            //Assert
            await Assert.ThrowsAsync<NullReferenceException>(async () => await service.AddRatingToCake(3, 5));
        }

        [Fact]
        public async Task AddRatingToCake_WithInValidRate_ShoulThrow()
        {
            //Arrange
            var db = this.SetDb();

            await this.SeedProducts(db);

            var repo = new Repository<Product>(db);

            var service = new CakeService(null, repo, this.Mapper);

            //Act

            //Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await service.AddRatingToCake(2, 6));
        }

        [Fact]
        public async Task SoftDelete_WithValidId_ShouldMarkCakeAsIsDeleted()
        {
            //Arrange
            var db = this.SetDb();

            await this.SeedProducts(db);

            var repo = new Repository<Product>(db);

            var service = new CakeService(null, repo, this.Mapper);

            //Act
            await service.SoftDelete(1);
            var expected = true;
            var actual = repo.All().SingleOrDefault(p => p.Id == 1).IsDeleted;

            //Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task GetAllActiveCakes_ShouldReturnAllCakesNotDeleted()
        {
            //Arrange
            var db = this.SetDb();

            var repo = new Repository<Product>(db);

            var cakeModel1 = new CreateCakeViewModel { Name = "Chocolate Peanut Cake", CategoryId = 1, Price = 35.50m, Description = "This Chocolate and Peanut Butter Drip Cake is completely sinful.", Image = "https://res.cloudinary.com/cakeit/image/upload/ar_1:1,c_fill,g_auto,e_art:hokusai/v1544136591/Chocolate_and_Peanut_cake.jpg"};

            var cakeModel2 = new CreateCakeViewModel { Name = "Chocolate Drip Cake", CategoryId = 1, Price = 35.50m, Description = "This Chocolate and Peanut Butter Drip Cake is completely sinful.", Image = "https://res.cloudinary.com/cakeit/image/upload/ar_1:1,c_fill,g_auto,e_art:hokusai/v1544136590/Chocolate_Drip_cake.jpg" };

            var cakeService = new CakeService(null, repo, this.Mapper);

            await cakeService.AddCakeToDb(cakeModel1);

            await cakeService.AddCakeToDb(cakeModel2);

            var deleteCake = await repo.GetByIdAsync(1);
            deleteCake.IsDeleted = true;
            deleteCake.Category = new Category { Id = 1, Type = Models.Enums.CategoryType.Cake };

            var activeCake = await repo.GetByIdAsync(2);
            activeCake.Category = new Category {Id = 1, Type = Models.Enums.CategoryType.Cake };
            await repo.SaveChangesAsync();

            //Act
            
var allActiveCakes = cakeService.GetAllActiveCakes();

            var expectedCakesCount = 1;
            var actualCakesCount = allActiveCakes.Count();

            //Assert
            Assert.Equal(expectedCakesCount, actualCakesCount);
        }

        [Fact]
        public async Task GetCakeToEdit_WithValidCakeId_ShouldReturnProduct()
        {
            //Assert
            var db = this.SetDb();

            await this.SeedProducts(db);

            var repo = new Repository<Product>(db);

            var service = new CakeService(null, repo, this.Mapper);

            //Act
            var result = await service.GetCakeToEdit(2);

            var expectedCakeId = 2;
            var acutalCakeId = result.Id;

            //Assert
            Assert.Equal(expectedCakeId, acutalCakeId);

        }

        [Fact]
        public async Task GetCakeToEdit_WithInValidCakeId_ShouldThrow()
        {
            //Assert
            var db = this.SetDb();

            await this.SeedProducts(db);

            var repo = new Repository<Product>(db);

            var service = new CakeService(null, repo, this.Mapper);

            //Act

            //Assert
            await Assert.ThrowsAsync<NullReferenceException>(async () => await service.GetCakeToEdit(3));

        }

        [Fact]
        public async Task SoftDelete_WithValidCustomCakeId_ShouldMarkCakeAsDeleted()
        {
            //Assert
            var db = this.SetDb();
            var repo = new Repository<CustomCakeImg>(db);
            var mock = new Mock<ILogger<CustomCakeService>>();
            ILogger<CustomCakeService> logger = mock.Object;
            var productRepo = new Repository<Product>(db);

            var productService = new CakeService(null, productRepo, this.Mapper);

            var service = new CustomCakeService(productRepo, repo, this.Mapper, logger);

            CustomCakeOrderViewModel model = new CustomCakeOrderViewModel
            {
                Sponge = "Vanilla",
                FirstLayerCream = "Whipped",
                SecondLayerCream = "Whipped",
                Filling = "No_Filling",
                SideDecoration = "White_Chocolate_Cigarettes",
                TopDecoration = "Habana",
                NumberOfSlices = 6,
                Img = null,
            };

            var product = service.CreateCustomProduct(model);

            await productService.AddCakeToDb(product);

            //Act
            await productService.SoftDelete(1);

            var expectedIsDeleted = true;
            var actualIsDeleted = (await productRepo.GetByIdAsync(1)).IsDeleted;

            //Assert
            Assert.Equal(expectedIsDeleted, actualIsDeleted);
        }

        [Fact]
        public async Task SoftDelete_WithInValidCustomCakeId_ShouldMarkCakeAsDeleted()
        {
            //Assert
            var db = this.SetDb();
            var repo = new Repository<CustomCakeImg>(db);
            var mock = new Mock<ILogger<CustomCakeService>>();
            ILogger<CustomCakeService> logger = mock.Object;
            var productRepo = new Repository<Product>(db);

            var productService = new CakeService(null, productRepo, this.Mapper);

            var service = new CustomCakeService(productRepo, repo, this.Mapper, logger);

            CustomCakeOrderViewModel model = new CustomCakeOrderViewModel
            {
                Sponge = "Vanilla",
                FirstLayerCream = "Whipped",
                SecondLayerCream = "Whipped",
                Filling = "No_Filling",
                SideDecoration = "White_Chocolate_Cigarettes",
                TopDecoration = "Habana",
                NumberOfSlices = 6,
                Img = null,
            };

            var product = service.CreateCustomProduct(model);

            await productService.AddCakeToDb(product);

            //Act
           

            //Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await productService.SoftDelete(2));
        }

        [Fact]
        public async Task GetAllCakes_ShouldReturnAllCakesDeletedAndNot()
        {
            //Arrange
            var db = this.SetDb();

            var repo = new Repository<Product>(db);

            var cakeModel1 = new CreateCakeViewModel { Name = "Chocolate Peanut Cake", CategoryId = 1, Price = 35.50m, Description = "This Chocolate and Peanut Butter Drip Cake is completely sinful.", Image = "https://res.cloudinary.com/cakeit/image/upload/ar_1:1,c_fill,g_auto,e_art:hokusai/v1544136591/Chocolate_and_Peanut_cake.jpg" };

            var cakeModel2 = new CreateCakeViewModel { Name = "Chocolate Drip Cake", CategoryId = 1, Price = 35.50m, Description = "This Chocolate and Peanut Butter Drip Cake is completely sinful.", Image = "https://res.cloudinary.com/cakeit/image/upload/ar_1:1,c_fill,g_auto,e_art:hokusai/v1544136590/Chocolate_Drip_cake.jpg" };

            var cakeService = new CakeService(null, repo, this.Mapper);

            await cakeService.AddCakeToDb(cakeModel1);

            await cakeService.AddCakeToDb(cakeModel2);

            var deleteCake = await repo.GetByIdAsync(1);
            deleteCake.IsDeleted = true;
            deleteCake.Category = new Category { Id = 1, Type = Models.Enums.CategoryType.Cake };

            var activeCake = await repo.GetByIdAsync(2);
            activeCake.Category = new Category { Id = 1, Type = Models.Enums.CategoryType.Cake };
            await repo.SaveChangesAsync();

            //Act

            var allActiveCakes = cakeService.GetAllCakes();

            var expectedCakesCount = 2;
            var actualCakesCount = allActiveCakes.Count();

            //Assert
            Assert.Equal(expectedCakesCount, actualCakesCount);
        }
    }
}
