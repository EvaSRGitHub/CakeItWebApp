using AutoMapper;
using CakeItWebApp.Data;
using CakeItWebApp.Models;
using CakeItWebApp.Services.Common.Repository;
using CakeItWebApp.ViewModels.CustomCake;
using CakeWebApp.Services.Common.CommonServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CakeItWebApp.Services.Common.Tests
{
    public class CustomCakeServiceTests:BaseServiceTestClass
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

        private async Task SeedCustomCakeImg(CakeItDbContext db)
        {
            var repo = new Repository<CustomCakeImg>(db);
            var mock = new Mock<ILogger<CustomCakeService>>();
            ILogger<CustomCakeService> logger = mock.Object;
            var service = new CustomCakeService(null, repo, this.Mapper, logger);

            CustomCakeImgViewModel model1 = new CustomCakeImgViewModel
            {
                Side = "White Cigarettes",
                Top = "Habana",
                Name = "Habana" + " " + "White Cigarettes",
                Img = "https://someurl.bg"
            };

            await service.AddCustomCakeImg(model1);

            CustomCakeImgViewModel model2 = new CustomCakeImgViewModel
            {
                Side = "Dark Cigarettes",
                Top = "Meksiko",
                Name = "Meksiko" + " " + "Dark Cigarettes",
                Img = "https://otherurl.bg"
            };

            await service.AddCustomCakeImg(model2);

            await repo.SaveChangesAsync();
        }

        [Fact]
        public async Task AddCustomCakeImg_ShouldAddDataToCustomCakeImgTable()
        {
            //Arrange
            var db = this.SetDb();

            var repo = new Repository<CustomCakeImg>(db);

            var mock = new Mock<ILogger<CustomCakeService>>();

            ILogger<CustomCakeService> logger = mock.Object;

            var service = new CustomCakeService(null, repo, this.Mapper, logger);

            CustomCakeImgViewModel model = new CustomCakeImgViewModel
            {
                Side = "White Cigarettes",
                Top = "Habana",
                Name = "Habana"  + " " + "White Cigarettes",
                Img = "https://res.cloudinary.com/cakeit/image/upload/v1545083551/Top_Habana_WhiteCigarettes.png"
            };

            //Act
            await service.AddCustomCakeImg(model);
            var expected = "Habana White Cigarettes";
            var actual = repo.All().First().Name;

            //Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task AddCustomCakeImg_WithDuplicateCakeName_ShouldThrow()
        {
            //Arrange
            var db = this.SetDb();

            var repo = new Repository<CustomCakeImg>(db);
            var mock = new Mock<ILogger<CustomCakeService>>();
            ILogger<CustomCakeService> logger = mock.Object;
            var service = new CustomCakeService(null, repo, this.Mapper, logger);

            CustomCakeImgViewModel model1 = new CustomCakeImgViewModel
            {
                Side = "White Cigarettes",
                Top = "Habana",
                Name = "Habana" + " " + "White Cigarettes",
                Img = "https://res.cloudinary.com/cakeit/image/upload/v1545083551/Top_Habana_WhiteCigarettes.png"
            };

            CustomCakeImgViewModel model2 = new CustomCakeImgViewModel
            {
                Side = "White Cigarettes",
                Top = "Habana",
                Name = "Habana" + " " + "White Cigarettes",
                Img = "https://res.cloudinary.com/cakeit/image/upload/v1545083551/Top_Habana_WhiteCigarettes.png"
            };
            await service.AddCustomCakeImg(model1);
            
            //Act

            //Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await service.AddCustomCakeImg(model2));
        }

        [Fact]
        public async Task AssignImgAndPrice_ShouldReturnModelWithPriceAndJmg()
        {
            //Arrange
            var db = this.SetDb();
            await this.SeedCustomCakeImg(db);
            var repo = new Repository<CustomCakeImg>(db);
            var mock = new Mock<ILogger<CustomCakeService>>();
            ILogger<CustomCakeService> logger = mock.Object;
            var service = new CustomCakeService(null, repo, this.Mapper, logger);

            CustomCakeOrderViewModel model = new CustomCakeOrderViewModel
            {
                Sponge = "Vanilla",
                FirstLayerCream = "Whipped",
                SecondLayerCream = "Whipped",
                Filling = "NoFilling",
                SideDecoration = "White Cigarettes",
                TopDecoration = "Habana",
                NumberOfSlices = 6,
                Img = null,
            };

            //Act
            var result = service.AssignImgAndPrice(model);

            var expectedPrice = 6 * 2.75m;
            var actualPrice = result.Price;

            //Assert
            Assert.Equal(expectedPrice, actualPrice);
            Assert.NotNull(result.Img);
        }

        [Fact]
        public async Task AssignImgAndPrice_WithNotCorrectlyFillInForm_ShouldThrow()
        {
            //Arrange
            var db = this.SetDb();
            await this.SeedCustomCakeImg(db);
            var repo = new Repository<CustomCakeImg>(db);
            var mock = new Mock<ILogger<CustomCakeService>>();
            ILogger<CustomCakeService> logger = mock.Object;
            var service = new CustomCakeService(null, repo, this.Mapper, logger);

            CustomCakeOrderViewModel model = new CustomCakeOrderViewModel
            {
                Sponge = "Vanilla",
                FirstLayerCream = "Whipped",
                SecondLayerCream = "--Choose Second Layer Cream--",
                Filling = "NoFilling",
                SideDecoration = "White Cigarettes",
                TopDecoration = "Habana",
                NumberOfSlices = 6,
                Img = null,
            };

            //Act

            //Assert
            Assert.Throws<InvalidOperationException>(() => service.AssignImgAndPrice(model));
        }

        [Fact]
        public async Task AssignImgAndPrice_WithInvalidSide_ShouldThrow()
        {
            //Arrange
            var db = this.SetDb();
            await this.SeedCustomCakeImg(db);
            var repo = new Repository<CustomCakeImg>(db);
            var mock = new Mock<ILogger<CustomCakeService>>();
            ILogger<CustomCakeService> logger = mock.Object;
            var service = new CustomCakeService(null, repo, this.Mapper, logger);

            CustomCakeOrderViewModel model = new CustomCakeOrderViewModel
            {
                Sponge = "Vanilla",
                FirstLayerCream = "Whipped",
                SecondLayerCream = "Whipped",
                Filling = "NoFilling",
                SideDecoration = "White Dots",
                TopDecoration = "Habana",
                NumberOfSlices = 6,
                Img = null,
            };

            //Act

            //Assert
            Assert.Throws<NullReferenceException>(() => service.AssignImgAndPrice(model));
        }

        [Fact]
        public async Task AssignImgAndPrice_WhithInvalidImgUrn_ShouldThrow()
        {
            //Arrange
            var db = this.SetDb();

            var repo = new Repository<CustomCakeImg>(db);
            var mock = new Mock<ILogger<CustomCakeService>>();
            ILogger<CustomCakeService> logger = mock.Object;
            var service = new CustomCakeService(null, repo, this.Mapper, logger);

            CustomCakeImgViewModel imgModel = new CustomCakeImgViewModel
            {
                Side = "White Cigarettes",
                Top = "Habana",
                Name = "Habana" + " " + "White Cigarettes",
                Img = "null"
            };

            await service.AddCustomCakeImg(imgModel);

            CustomCakeOrderViewModel model = new CustomCakeOrderViewModel
            {
                Sponge = "Vanilla",
                FirstLayerCream = "Whipped",
                SecondLayerCream = "Whipped",
                Filling = "NoFilling",
                SideDecoration = "White Cigarettes",
                TopDecoration = "Habana",
                NumberOfSlices = 6,
                Img = null,
            };

            //Act

            //Assert
            Assert.Throws<InvalidOperationException>(() => service.AssignImgAndPrice(model));
        }

        [Fact]
        public void CreateCustomProduct_ShouldReturnProduct()
        {
            //Arrange
            var db = this.SetDb();
            var repo = new Repository<CustomCakeImg>(db);
            var mock = new Mock<ILogger<CustomCakeService>>();
            ILogger<CustomCakeService> logger = mock.Object;
            var service = new CustomCakeService(null, repo, this.Mapper, logger);

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

            //Act
            var product = service.CreateCustomProduct(model);

            //Assert
            Assert.NotNull(product);
            Assert.IsType<Product>(product);
        }

        [Fact]
        public void CreateCustomProduct_WithInValidIngredients_ShouldThrow()
        {
            //Arrange
            var db = this.SetDb();
            var repo = new Repository<CustomCakeImg>(db);
            var mock = new Mock<ILogger<CustomCakeService>>();
            ILogger<CustomCakeService> logger = mock.Object;
            var service = new CustomCakeService(null, repo, this.Mapper, logger);

            CustomCakeOrderViewModel model = new CustomCakeOrderViewModel
            {
                Sponge = "Vanilla",
                FirstLayerCream = "Whipped",
                SecondLayerCream = "Whipped",
                Filling = "NoFilling",
                SideDecoration = "White_Chocolate_Cigarettes",
                TopDecoration = "Habana",
                NumberOfSlices = 6,
                Img = null,
            };

            //Act

            //Assert
            Assert.Throws<AutoMapperMappingException
>(() => service.CreateCustomProduct(model));
        }

        [Fact]
        public void CreateCustomProduct_WithNullIngredients_ShouldThrow()
        {
            //Arrange
            var db = this.SetDb();
            var repo = new Repository<CustomCakeImg>(db);
            var mock = new Mock<ILogger<CustomCakeService>>();
            ILogger<CustomCakeService> logger = mock.Object;
            var service = new CustomCakeService(null, repo, this.Mapper, logger);

            CustomCakeOrderViewModel model = null;

            //Act

            //Assert
            Assert.Throws<AutoMapperMappingException
>(() => service.CreateCustomProduct(model));
        }

        

        [Fact]
        public async Task UpdateCustomCakeImg_WithValidData_ShouldSaveUpdatedEntry()
        {
            //Arrange
            var db = this.SetDb();

            await this.SeedCustomCakeImg(db);

            var repo = new Repository<CustomCakeImg>(db);
            var mock = new Mock<ILogger<CustomCakeService>>();
            ILogger<CustomCakeService> logger = mock.Object;
            var service = new CustomCakeService(null, repo, this.Mapper, logger);

            var customCakeImg = await repo.GetByIdAsync(2);
            db.Entry(customCakeImg).State = EntityState.Detached;
            var model = this.Mapper.Map<CustomCakeImg, CustomCakeImgViewModel>(customCakeImg);
            model.Name = "Golden leaf";

            //Act
            await service.UpdateCustomCakeImg(model);

            var expectedTitle = "Golden leaf";
            var actualTitle = repo.All().SingleOrDefault(t => t.Id == 2).Name;

            //Assert
            Assert.Equal(expectedTitle, actualTitle);
        }

        [Fact]
        public async Task UpdateCustomCakeImg_WithDuplicateTitle_ShouldSaveUpdatedTurorialThrow()
        {
            //Arrange
            var db = this.SetDb();

            await this.SeedCustomCakeImg(db);

            var repo = new Repository<CustomCakeImg>(db);
            var mock = new Mock<ILogger<CustomCakeService>>();
            ILogger<CustomCakeService> logger = mock.Object;
            var service = new CustomCakeService(null, repo, this.Mapper, logger);

            var customCakeImg = await repo.GetByIdAsync(2);
            db.Entry(customCakeImg).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
            var model = this.Mapper.Map<CustomCakeImg, CustomCakeImgViewModel>(customCakeImg);
            model.Name = "Habana White Cigarettes";

            //Act

            //Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await service.UpdateCustomCakeImg(model));
        }

        [Fact]
        public async Task UpdateCustomCakeImg_WithDuplicateImgUrl_ShouldSaveUpdatedTurorialThrow()
        {
            //Arrange
            var db = this.SetDb();

            await this.SeedCustomCakeImg(db);

            var repo = new Repository<CustomCakeImg>(db);
            var mock = new Mock<ILogger<CustomCakeService>>();
            ILogger<CustomCakeService> logger = mock.Object;
            var service = new CustomCakeService(null, repo, this.Mapper, logger);

            var customCakeImg = await repo.GetByIdAsync(2);
           db.Entry(customCakeImg).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
            var model = this.Mapper.Map<CustomCakeImg, CustomCakeImgViewModel>(customCakeImg);
            model.Img = "https://someurl.bg";

            //Act

            //Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await service.UpdateCustomCakeImg(model));
        }

        [Fact]
        public async Task DeleteCustomCakeImg_WithValidiD_ShouldDelete()
        {
            //Arrange
            var db = this.SetDb();

            await this.SeedCustomCakeImg(db);

            var repo = new Repository<CustomCakeImg>(db);
            var mock = new Mock<ILogger<CustomCakeService>>();
            ILogger<CustomCakeService> logger = mock.Object;
            var service = new CustomCakeService(null, repo, this.Mapper, logger);
            var cake = await service.GetCustomCakeImgById(1);
            //Act
            await service.DeleteCustomCakeImg(cake);
            var expectedRepoCount = 1;
            var actualRepoCount = repo.All().Count();

            var expectedTutorialId = 2;
            var actualTutorialId = repo.All().First().Id;

            //Asser
            Assert.Equal(expectedRepoCount, actualRepoCount);
            Assert.Equal(expectedTutorialId, actualTutorialId);
        }

        [Fact]
        public async Task DeleteCustomCakeImg_WithInValidiD_ShouldThrow()
        {
            //Arrange
            var db = this.SetDb();

            await this.SeedCustomCakeImg(db);

            var repo = new Repository<CustomCakeImg>(db);
            var mock = new Mock<ILogger<CustomCakeService>>();
            ILogger<CustomCakeService> logger = mock.Object;
            var service = new CustomCakeService(null, repo, this.Mapper, logger);

            //Act
            
            //Asser
            await Assert.ThrowsAsync<NullReferenceException>(async () => await service.DeleteCustomCakeImg(await service.GetCustomCakeImgById(3)));
        }

        [Fact]
        public async Task GetCustomCakeImglById_WithValidId_ShouldReturnModel()
        {
            //Arrange
            var db = this.SetDb();

            await this.SeedCustomCakeImg(db);

            var repo = new Repository<CustomCakeImg>(db);
            var mock = new Mock<ILogger<CustomCakeService>>();
            ILogger<CustomCakeService> logger = mock.Object;
            var service = new CustomCakeService(null, repo, this.Mapper, logger);

            //Act
            var model = await service.GetCustomCakeImgById(2);

            var expectedId = 2;
            var actualId = model.Id;

            //Assert
            Assert.NotNull(model);
            Assert.Equal(expectedId, actualId);
        }

        [Fact]
        public async Task GetCustomCakeImglById_WithInValidId_ShouldThrow()
        {
            //Arrange
            var db = this.SetDb();

            await this.SeedCustomCakeImg(db);

            var repo = new Repository<CustomCakeImg>(db);
            var mock = new Mock<ILogger<CustomCakeService>>();
            ILogger<CustomCakeService> logger = mock.Object;
            var service = new CustomCakeService(null, repo, this.Mapper, logger);

            //Act

            //Assert
            await Assert.ThrowsAsync<NullReferenceException>(async () => await service.GetCustomCakeImgById(3));
        }

        [Fact]
        public async Task GetAllCustomCakesImg_ShouldReturnCollection()
        {
            //Arrange
            var db = this.SetDb();

            await this.SeedCustomCakeImg(db);

            var repo = new Repository<CustomCakeImg>(db);
            var mock = new Mock<ILogger<CustomCakeService>>();
            ILogger<CustomCakeService> logger = mock.Object;
            var service = new CustomCakeService(null, repo, this.Mapper, logger);

            //Act
            var result = service.GetAllCustomCakesImg();

            var expectedCount = 2;
            var actualCount = result.Count();

            //Assert
            Assert.Equal(expectedCount, actualCount);
        }
    }
}
