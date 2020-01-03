using CakeItWebApp.Data;
using CakeItWebApp.Models;
using CakeItWebApp.Services.Common.Repository;
using CakeItWebApp.ViewModels.Cakes;
using CakeItWebApp.ViewModels.CustomCake;
using CakeWebApp.Services.Common.Cart;
using CakeWebApp.Services.Common.CommonServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CakeItWebApp.Services.Common.Tests
{
    public class CartServiceTests : BaseServiceTestClass
    {
        private string userName = "test@test.bg";
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

        private class TestCartSessionWrapper : ICartSessionWrapper
        {
            private IList<Item> session;

            public TestCartSessionWrapper()
            {
                this.session = new List<Item>();
            }

            public IList<Item> GetJson()
            {
                return this.session;
            }

            public void SetJson(object value)
            {
                this.session = value as List<Item>;
            }
        }

        [Fact]
        public async Task AddToCart_WithValidProductId_ShouldAddNewItemToCart()
        {
            //Arrange 
            var db = this.SetDb();
            await this.SeedProducts(db);

            var productRepo = new Repository<Product>(db);
            var wrapper = new TestCartSessionWrapper();
            var cartManager = new CartManager(wrapper);

            var cartService = new CartService(productRepo, cartManager);

            //Act
            await cartService.AddToCart(1);
            var expectedNumber = 1;
            var expectedItemName = "Chocolate Peanut Cake";
            var actualItemName = cartManager.GetCartItem().First().Product.Name;
            var actualNumber = cartManager.GetCartItem().Count;

            //Assert
            Assert.Equal(expectedNumber, actualNumber);
            Assert.Equal(expectedItemName, actualItemName);
        }

        [Fact]
        public async Task AddToCart_WhitInValidProductId_ShouldThrow()
        {
            //Arrange
            var db = this.SetDb();
            await this.SeedProducts(db);

            var productRepo = new Repository<Product>(db);
            var wrapper = new TestCartSessionWrapper();
            var cartManager = new CartManager(wrapper);

            var cartService = new CartService(productRepo, cartManager);

            //Assert
            await Assert.ThrowsAsync<NullReferenceException>(async () => await cartService.AddToCart(3));
        }

        [Fact]
        public async Task AddToCart_WhitInValidProductId_ShouldNotAddItem()
        {
            //Arrange
            var db = this.SetDb();
            await this.SeedProducts(db);

            var productRepo = new Repository<Product>(db);
            var wrapper = new TestCartSessionWrapper();
            var cartManager = new CartManager(wrapper);

            var cartService = new CartService(productRepo, cartManager);

            //Arrange
            try
            {
                await cartService.AddToCart(3);
            }
            catch (Exception)
            { }

            var expectedCartCount = 0;
            var actualCartCount = cartManager.GetCartItem().Count;

            //Assert
            Assert.Equal(expectedCartCount, actualCartCount);
        }

        [Fact]
        public async Task AddToCart_AddSecondTimeOneItem_ShouldIncreaseItemQuantity()
        {
            //Arrange
            var db = this.SetDb();
            await this.SeedProducts(db);

            var productRepo = new Repository<Product>(db);
            var wrapper = new TestCartSessionWrapper();
            var cartManager = new CartManager(wrapper);

            var cartService = new CartService(productRepo, cartManager);

            //Act
            await cartService.AddToCart(1);
            await cartService.AddToCart(1);

            var expected = 2;
            var actual = cartManager.GetCartItem().FirstOrDefault(i => i.Product.Id == 1).Quantity;

            //Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task GetCartItems_WithNoItems_ShouldReturnNull()
        {
            //Arrange
            var db = this.SetDb();
            await this.SeedProducts(db);

            var productRepo = new Repository<Product>(db);
            var wrapper = new TestCartSessionWrapper();
            var cartManager = new CartManager(wrapper);

            var cartService = new CartService(productRepo, cartManager);

            //Act
            var result = cartService.GetCartItems();

            var expectedResultCount = 0;
            var actualResultCount = result.CartItems.Count;

            //Assert
            Assert.Equal(expectedResultCount, actualResultCount);
        }

        [Fact]
        public async Task GetCartItems_WithTwoItems_ShouldReturnItemCount2()
        {
            //Arrange
            var db = this.SetDb();
            await this.SeedProducts(db);

            var productRepo = new Repository<Product>(db);
            var wrapper = new TestCartSessionWrapper();
            var cartManager = new CartManager(wrapper);

            var cartService = new CartService(productRepo, cartManager);

            await cartService.AddToCart(1);
            await cartService.AddToCart(2);

            //Act
            var result = cartService.GetCartItems();

            var expectedResultCount = 2;
            var actualResultCount = result.CartItems.Count;

            //Assert
            Assert.Equal(expectedResultCount, actualResultCount);
        }

        [Fact]
        public async Task RemoveFromCart_WithCartAmountingToTwo_ShouldReturnCartItemsCount1()
        {
            //Arrange
            var db = this.SetDb();
            await this.SeedProducts(db);

            var productRepo = new Repository<Product>(db);
            var wrapper = new TestCartSessionWrapper();
            var cartManager = new CartManager(wrapper);

            var cartService = new CartService(productRepo, cartManager);

            await cartService.AddToCart(1);
            await cartService.AddToCart(2);

            //Act
            await cartService.RemoveFromCart(1);

            var expected = 1;
            var actual = cartManager.GetCartItem().Count;

            //Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task RemoveFromCart_WithOneItem_ShouldReturnCartItemsCount0()
        {
            //Arrange
            var db = this.SetDb();
            await this.SeedProducts(db);

            var productRepo = new Repository<Product>(db);
            var wrapper = new TestCartSessionWrapper();
            var cartManager = new CartManager(wrapper);

            var cartService = new CartService(productRepo, cartManager);

            await cartService.AddToCart(1);

            //Act
            await cartService.RemoveFromCart(1);

            var expected = 0;
            var actual = cartManager.GetCartItem().Count;

            //Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task RemoveFromCart_WhitInValidProductId_ShouldThrow()
        {
            //Arrange
            var db = this.SetDb();
            await this.SeedProducts(db);

            var productRepo = new Repository<Product>(db);
            var wrapper = new TestCartSessionWrapper();
            var cartManager = new CartManager(wrapper);

            var cartService = new CartService(productRepo, cartManager);

            await cartService.AddToCart(1);

            //Assert
            await Assert.ThrowsAsync<NullReferenceException>(async () => await cartService.RemoveFromCart(3));
        }

        [Fact]
        public async Task RemoveFromCart_WithItemCategoryTwo_ShouldReturnEmptyCart()
        {
            //Arrange
            var db = this.SetDb();

            var repo = new Repository<CustomCakeImg>(db);
            var productRepo = new Repository<Product>(db);
            var productService = new CakeService(null, productRepo, this.Mapper);
            var customCakeService = new CustomCakeService(productRepo, repo, this.Mapper, null);

            var wrapper = new TestCartSessionWrapper();
            var cartManager = new CartManager(wrapper);

            var cartService = new CartService(productRepo, cartManager);

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

            var product = customCakeService.CreateCustomProduct(model);

            await productService.AddCakeToDb(product);

            await cartService.AddToCart(1);

            //Act
            await cartService.RemoveFromCart(1);

            //Assert
            Assert.Empty(cartManager.GetCartItem());
        }

        [Fact]
        public async Task EmptyCart_ShouldEmptyCart()
        {
            //Arange
            var db = this.SetDb();
            await this.SeedProducts(db);

            var productRepo = new Repository<Product>(db);
            var wrapper = new TestCartSessionWrapper();
            var cartManager = new CartManager(wrapper);

            var cartService = new CartService(productRepo, cartManager);

            await cartService.AddToCart(1);

            //Act
            cartService.EmptyCart();

            //Assert
            Assert.Empty(cartManager.GetCartItem());
        }
    }
}
