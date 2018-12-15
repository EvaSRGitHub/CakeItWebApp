using CakeItWebApp.Models;
using CakeItWebApp.Services.Common.Repository;
using CakeItWebApp.ViewModels.Cakes;
using CakeWebApp.Models;
using CakeWebApp.Services.Common.CommonServices;
using Microsoft.Extensions.DependencyInjection;
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
        [Fact]
        public async Task AddShoppingCartItem_WithValidProductId_ShouldAddNewItemToDb()
        {
            //Arrange
            await this.SeedProducts();
            var repo = new Repository<ShoppingCartItem>(this.Db);

            var productRepo = new Repository<Product>(this.Db);

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
            var repo = new Repository<ShoppingCartItem>(this.Db);
            var productRepo = new Repository<Product>(this.Db);

            var provider = new Mock<IServiceProvider>();

            var cart = new Mock<ShoppingCart>();

            var shoppingCartService = new ShoppingCartService(repo, productRepo, null, provider.Object, cart.Object);

            //Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await shoppingCartService.AddToShoppingCart(1));
        }

        [Fact]
        public async Task AddShoppingCartItem_WhitInValidProductId_ShouldNotAddItem()
        {
            //Arrange
            var repo = new Repository<ShoppingCartItem>(this.Db);
            var productRepo = new Repository<Product>(this.Db);

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
            await this.SeedProducts();

            var repo = new Repository<ShoppingCartItem>(this.Db);

            var productRepo = new Repository<Product>(this.Db);

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
            var repo = new Repository<ShoppingCartItem>(this.Db);

            var productRepo = new Repository<Product>(this.Db);

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

            await this.SeedProducts();

            var repo = new Repository<ShoppingCartItem>(this.Db);

            var productRepo = new Repository<Product>(this.Db);

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

            await this.SeedProducts();

            var repo = new Repository<ShoppingCartItem>(this.Db);

            var productRepo = new Repository<Product>(this.Db);

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
            await this.SeedProducts();

            var repo = new Repository<ShoppingCartItem>(this.Db);

            var productRepo = new Repository<Product>(this.Db);

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
            await this.SeedProducts();
            var repo = new Repository<ShoppingCartItem>(this.Db);

            var productRepo = new Repository<Product>(this.Db);

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
            var repo = new Repository<ShoppingCartItem>(this.Db);
            var productRepo = new Repository<Product>(this.Db);

            var provider = new Mock<IServiceProvider>();

            var cart = new Mock<ShoppingCart>();

            var shoppingCartService = new ShoppingCartService(repo, productRepo, null, provider.Object, cart.Object);

            //Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await shoppingCartService.RemoveFromShoppingCart(1));
        }
    }
}
