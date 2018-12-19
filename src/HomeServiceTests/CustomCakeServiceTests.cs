using AutoMapper;
using CakeItWebApp.Models;
using CakeItWebApp.Services.Common.Repository;
using CakeItWebApp.ViewModels.CustomCake;
using CakeWebApp.Services.Common.CommonServices;
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
            var service = new CustomCakeService(null, repo, this.Mapper);
            CustomCakeImgViewModel model = new CustomCakeImgViewModel
            {
                Side = "White Cigarettes",
                Top = "Habana",
                Name = "Habana" + " " + "White Cigarettes",
                Img = "https://res.cloudinary.com/cakeit/image/upload/v1545083551/Top_Habana_WhiteCigarettes.png"
            };

            await service.AddCustomCakeImg(model);
        }

        [Fact]
        public async Task AddCustomCakeImg_ShouldAddDataToCustomCakeImgTable()
        {
            //Arrange
            var repo = new Repository<CustomCakeImg>(this.Db);
            var service = new CustomCakeService(null, repo, this.Mapper);

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
            var service = new CustomCakeService(null, repo, this.Mapper);

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
            var service = new CustomCakeService(null, repo, this.Mapper);

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
            var service = new CustomCakeService(null, repo, this.Mapper);

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
            var service = new CustomCakeService(null, repo, this.Mapper);

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
            var service = new CustomCakeService(null, repo, this.Mapper);

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
            var repo = new Repository<CustomCakeImg>(this.Db);
            var productRepo = new Repository<Product>(this.Db);
            var service = new CustomCakeService(null, repo, this.Mapper);

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
            var repo = new Repository<CustomCakeImg>(this.Db);
            var productRepo = new Repository<Product>(this.Db);
            var service = new CustomCakeService(null, repo, this.Mapper);

            //Act

            //Assert
            Assert.Throws<NullReferenceException>(() => productRepo.All().LastOrDefault().Id);
        }
    }
}
