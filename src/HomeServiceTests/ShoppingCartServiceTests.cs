using CakeItWebApp.Data;
using CakeItWebApp.Models;
using CakeItWebApp.Services.Common.Repository;
using CakeItWebApp.ViewModels.Cakes;
using CakeItWebApp.ViewModels.CustomCake;
using CakeWebApp.Models;
using CakeWebApp.Services.Common.CommonServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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
    public class ShoppingCartServiceTests : BaseServiceTestClass
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
        public async Task AddShoppingCartItem_WithValidProductId_ShouldAddNewItemToDb()
        {
            //Arrange
            var db = this.SetDb();

            await this.SeedProducts(db);

            var repo = new Repository<ShoppingCartItem>(db);

            var productRepo = new Repository<Product>(db);

            var provider = new Mock<IServiceProvider>();

            var cart = new Mock<ShoppingCart>();

            var shoppingCartService = new ShoppingCartService(repo, productRepo, null, provider.Object, cart.Object);

            //Act
            await shoppingCartService.AddToShoppingCart(1);
            var expected = 1;
            var actual = repo.All().Count();

            var expectedItemName = "Chocolate Peanut Cake";
            var actualItemName = repo.All().Select(i => i.Product.Name).First();

            //Assert
            Assert.Equal(expected, actual);
            Assert.Equal(expectedItemName, actualItemName);
        }

        [Fact]
        public async Task AddShoppingCartItem_WhitInValidProductId_ShouldThrow()
        {
            //Arrange
            var db = this.SetDb();

            var repo = new Repository<ShoppingCartItem>(db);

            var productRepo = new Repository<Product>(db);

            var provider = new Mock<IServiceProvider>();

            var cart = new Mock<ShoppingCart>();

            var shoppingCartService = new ShoppingCartService(repo, productRepo, null, provider.Object, cart.Object);

            //Assert
            await Assert.ThrowsAsync<NullReferenceException>(async () => await shoppingCartService.AddToShoppingCart(1));
        }

        [Fact]
        public async Task AddShoppingCartItem_WhitInValidProductId_ShouldNotAddItem()
        {
            //Arrange
            var db = this.SetDb();

            var repo = new Repository<ShoppingCartItem>(db);

            var productRepo = new Repository<Product>(db);


            var provider = new Mock<IServiceProvider>();

            var cart = new Mock<ShoppingCart>();

            var shoppingCartService = new ShoppingCartService(repo, productRepo, null, provider.Object, cart.Object);

            //Arrange
            try
            {
                await shoppingCartService.AddToShoppingCart(1);
            }
            catch (Exception)
            {

            }

            var expectedDbSetCount = 0;
            var actualDbSetCount = repo.All().Count();

            //Assert
            Assert.Equal(expectedDbSetCount, actualDbSetCount);
        }

        [Fact]
        public async Task AddShoppingCartItem_AddSecondTimeOneItem_ShouldIncreaseItemQuantityInDb()
        {
            //Arrange
            var db = this.SetDb();

            await this.SeedProducts(db);

            var repo = new Repository<ShoppingCartItem>(db);

            var productRepo = new Repository<Product>(db);

            var provider = new Mock<IServiceProvider>();

            var cart = new Mock<ShoppingCart>();

            var shoppingCartService = new ShoppingCartService(repo, productRepo, null, provider.Object, cart.Object);

            //Act
            await shoppingCartService.AddToShoppingCart(1);
            await shoppingCartService.AddToShoppingCart(1);

            var expected = 2;
            var actual = repo.All().FirstOrDefault(i => i.ProductId == 1).Quantity;

            //Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetCartItems_WithNoItems_ShouldReturnNull()
        {
            //Arrange
            var db = this.SetDb();

            var repo = new Repository<ShoppingCartItem>(db);

            var productRepo = new Repository<Product>(db);

            var provider = new Mock<IServiceProvider>();

            var cart = new Mock<ShoppingCart>();

            var shoppingCartService = new ShoppingCartService(repo, productRepo, null, provider.Object, cart.Object);

            //Act
            var result = shoppingCartService.GetCartItems();

            var expectedResultCount = 0;
            var actualResultCount = result.Count();

            //Assert
            Assert.Equal(expectedResultCount, actualResultCount);
        }

        [Fact]
        public async Task GetCartItems_WithTwoItems_ShouldReturnItemCount2()
        {
            //Arrange
            var db = this.SetDb();

            await this.SeedProducts(db);

            var repo = new Repository<ShoppingCartItem>(db);

            var productRepo = new Repository<Product>(db);

            var provider = new Mock<IServiceProvider>();

            var cart = new Mock<ShoppingCart>();

            var shoppingCartService = new ShoppingCartService(repo, productRepo, null, provider.Object, cart.Object);

            await shoppingCartService.AddToShoppingCart(1);
            await shoppingCartService.AddToShoppingCart(2);

            //Act
            var result = shoppingCartService.GetCartItems();

            var expectedResultCount = 2;
            var actualResultCount = result.Count();

            //Assert
            Assert.Equal(expectedResultCount, actualResultCount);
        }

        [Fact]
        public async Task ClearShopingCart()
        {
            //Arrange
            var db = this.SetDb();

            await this.SeedProducts(db);

            var repo = new Repository<ShoppingCartItem>(db);

            var productRepo = new Repository<Product>(db);

            var provider = new Mock<IServiceProvider>();

            var cart = new Mock<ShoppingCart>();

            var shoppingCartService = new ShoppingCartService(repo, productRepo, null, provider.Object, cart.Object);

            await shoppingCartService.AddToShoppingCart(1);
            await shoppingCartService.AddToShoppingCart(2);

            //Act
            await shoppingCartService.ClearShoppingCart();

            var expected = 0;
            var actual = repo.All().Count();

            //Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task RemoveFromShoppingCart_WithItemAmountingToTwo_ShouldReturnCartItemsCount1()
        {
            //Arrange
            var db = this.SetDb();

            await this.SeedProducts(db);

            var repo = new Repository<ShoppingCartItem>(db);

            var productRepo = new Repository<Product>(db);

            var provider = new Mock<IServiceProvider>();

            var cart = new Mock<ShoppingCart>();

            var shoppingCartService = new ShoppingCartService(repo, productRepo, null, provider.Object, cart.Object);

            await shoppingCartService.AddToShoppingCart(1);
            await shoppingCartService.AddToShoppingCart(1);

            //Act
            await shoppingCartService.RemoveFromShoppingCart(1);

            var expected = 1;
            var actual = repo.All().Count();

            //Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task RemoveFromShoppingCart_WithOneItem_ShouldReturnCartItemsCount0()
        {
            //Arrange
            var db = this.SetDb();

            await this.SeedProducts(db);

            var repo = new Repository<ShoppingCartItem>(db);

            var productRepo = new Repository<Product>(db);

            var provider = new Mock<IServiceProvider>();

            var cart = new Mock<ShoppingCart>();

            var shoppingCartService = new ShoppingCartService(repo, productRepo, null, provider.Object, cart.Object);

            await shoppingCartService.AddToShoppingCart(1);

            //Act
            await shoppingCartService.RemoveFromShoppingCart(1);

            var expected = 0;
            var actual = repo.All().Count();

            //Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task RemoveFromShoppingCart_WhitInValidProductId_ShouldThrow()
        {
            //Arrange
            var db = this.SetDb();

            var repo = new Repository<ShoppingCartItem>(db);

            var productRepo = new Repository<Product>(db);

            var provider = new Mock<IServiceProvider>();

            var cart = new Mock<ShoppingCart>();

            var shoppingCartService = new ShoppingCartService(repo, productRepo, null, provider.Object, cart.Object);

            //Assert
            await Assert.ThrowsAsync<NullReferenceException>(async () => await shoppingCartService.RemoveFromShoppingCart(1));
        }

        [Fact]
        public async Task RemoveFromShoppingCart_WithOneItemCategoryTwo_ShouldReturnCartItemsCount0()
        {
            //Arrange
            var db = this.SetDb();

            var repo = new Repository<CustomCakeImg>(db);
            var mock = new Mock<ILogger<CustomCakeService>>();
            ILogger<CustomCakeService> logger = mock.Object;

            var productRepo = new Repository<Product>(db);
            var productService = new CakeService(null, productRepo, this.Mapper);

            var service = new CustomCakeService(productRepo, repo, this.Mapper, logger);

            var shoppingCartRepo = new Repository<ShoppingCartItem>(db);

            var provider = new Mock<IServiceProvider>();
            var cart = new Mock<ShoppingCart>();

            var shoppingCartService = new ShoppingCartService(shoppingCartRepo, productRepo, null, provider.Object, cart.Object);

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

            await shoppingCartService.AddToShoppingCart(1);

            //Act
            await shoppingCartService.RemoveFromShoppingCart(1);

            //Assert
            Assert.Empty(productRepo.All());
        }   

        [Fact]
        public async Task GetShoppingCart_ShouldReturnShoppingCart()
        {
            //Arrange
            var db = this.SetDb();

            await this.SeedProducts(db);

            var repo = new Repository<ShoppingCartItem>(db);

            var productRepo = new Repository<Product>(db);

            var provider = new Mock<IServiceProvider>();

            var cart = new Mock<ShoppingCart>();

            var shoppingCartService = new ShoppingCartService(repo, productRepo, null, provider.Object, cart.Object);

            //Act
            var shopCart = shoppingCartService.GetShoppingCart();

            //Assert
            Assert.NotNull(shopCart);
        }
    }
}
