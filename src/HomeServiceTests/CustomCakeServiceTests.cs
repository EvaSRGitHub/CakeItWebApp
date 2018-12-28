using AutoMapper;
using CakeItWebApp.Models;
using CakeItWebApp.Services.Common.Repository;
using CakeItWebApp.ViewModels.CustomCake;
using CakeWebApp.Services.Common.CommonServices;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CakeItWebApp.Services.Common.Tests
{
    public class CustomCakeServiceTests:BaseServiceTestClass
    {
        private async Task SeedCustomCakeImg()
        {
            var repo = new Repository<CustomCakeImg>(this.Db);
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
        }

        [Fact]
        public async Task AddCustomCakeImg_ShouldAddDataToCustomCakeImgTable()
        {
            //Arrange
            var repo = new Repository<CustomCakeImg>(this.Db);
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
            var repo = new Repository<CustomCakeImg>(this.Db);
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
            await this.SeedCustomCakeImg();
            var repo = new Repository<CustomCakeImg>(this.Db);
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
        public async Task AssignImgAndPrice_WhithInvalidImgUrn_ShouldThrow()
        {
            //Arrange
            var repo = new Repository<CustomCakeImg>(this.Db);
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
            var repo = new Repository<CustomCakeImg>(this.Db);
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
            var repo = new Repository<CustomCakeImg>(this.Db);
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
        public async Task GetProductId_ShouldReturnTheIdOfTheLastAddedProduct()
        {
            //Arrange
            var productRepo = new Repository<Product>(this.Db);
            var repo = new Repository<CustomCakeImg>(this.Db);
            var mock = new Mock<ILogger<CustomCakeService>>();
            ILogger<CustomCakeService> logger = mock.Object;
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
                Img = "https://res.cloudinary.com/cakeit/image/upload/v1545083551/Top_Habana_WhiteCigarettes.png"
            };

            var product = service.CreateCustomProduct(model);

            productRepo.Add(product);
            await productRepo.SaveChangesAsync();

            //Act
            int? result = productRepo.All().LastOrDefault().Id;

            //Assert
            Assert.NotNull(result);

        }

        [Fact]
        public void GetProductId_WithEmptyProductDb_ShouldThrow()
        {
            //Arrange
            var productRepo = new Repository<Product>(this.Db);
            var repo = new Repository<CustomCakeImg>(this.Db);
            var mock = new Mock<ILogger<CustomCakeService>>();
            ILogger<CustomCakeService> logger = mock.Object;
            var service = new CustomCakeService(productRepo, repo, this.Mapper, logger);

            //Act

            //Assert
            Assert.Throws<NullReferenceException>(() => productRepo.All().LastOrDefault().Id);
        }

        [Fact]
        public async Task UpdateCustomCakeImg_WithValidData_ShouldSaveUpdatedEntry()
        {
            //Arrange
            await this.SeedCustomCakeImg();
            var repo = new Repository<CustomCakeImg>(this.Db);
            var mock = new Mock<ILogger<CustomCakeService>>();
            ILogger<CustomCakeService> logger = mock.Object;
            var service = new CustomCakeService(null, repo, this.Mapper, logger);

            var customCakeImg = await repo.GetByIdAsync(2);
           this.Db.Entry(customCakeImg).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
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
            await this.SeedCustomCakeImg();
            var repo = new Repository<CustomCakeImg>(this.Db);
            var mock = new Mock<ILogger<CustomCakeService>>();
            ILogger<CustomCakeService> logger = mock.Object;
            var service = new CustomCakeService(null, repo, this.Mapper, logger);

            var customCakeImg = await repo.GetByIdAsync(2);
            this.Db.Entry(customCakeImg).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
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
            await this.SeedCustomCakeImg();
            var repo = new Repository<CustomCakeImg>(this.Db);
            var mock = new Mock<ILogger<CustomCakeService>>();
            ILogger<CustomCakeService> logger = mock.Object;
            var service = new CustomCakeService(null, repo, this.Mapper, logger);

            var customCakeImg = await repo.GetByIdAsync(2);
           this.Db.Entry(customCakeImg).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
            var model = this.Mapper.Map<CustomCakeImg, CustomCakeImgViewModel>(customCakeImg);
            model.Img = "https://someurl.bg";

            //Act

            //Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await service.UpdateCustomCakeImg(model));
        }

        [Fact]
        public async Task DeleteCustomCakeImg_WithValidiD_ShouldDeleteTheTutorial()
        {
            //Arrange
            await this.SeedCustomCakeImg();
            var repo = new Repository<CustomCakeImg>(this.Db);
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
        public async Task DeleteCustomCakeImg_WithInValidiD_ShouldDoNating()
        {
            //Arrange
            await this.SeedCustomCakeImg();
            var repo = new Repository<CustomCakeImg>(this.Db);
            var mock = new Mock<ILogger<CustomCakeService>>();
            ILogger<CustomCakeService> logger = mock.Object;
            var service = new CustomCakeService(null, repo, this.Mapper, logger);
            var cake = await service.GetCustomCakeImgById(3);

            //Act
            
            //Asser
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await service.DeleteCustomCakeImg(cake));
        }

        [Fact]
        public async Task GetCustomCakeImglById_WithValidId_ShouldReturnModel()
        {
            //Arrange
            await this.SeedCustomCakeImg();
            var repo = new Repository<CustomCakeImg>(this.Db);
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
            await this.SeedCustomCakeImg();
            var repo = new Repository<CustomCakeImg>(this.Db);
            var mock = new Mock<ILogger<CustomCakeService>>();
            ILogger<CustomCakeService> logger = mock.Object;
            var service = new CustomCakeService(null, repo, this.Mapper, logger);

            //Act

            //Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await service.GetCustomCakeImgById(3));
        }
    }
}
