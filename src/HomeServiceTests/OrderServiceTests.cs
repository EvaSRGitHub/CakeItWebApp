using CakeItWebApp.Models;
using CakeItWebApp.Services.Common.Repository;
using CakeItWebApp.ViewModels.Orders;
using CakeWebApp.Models;
using CakeWebApp.Services.Common.CommonServices;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CakeItWebApp.Services.Common.Tests
{
    public class OrderServiceTests:BaseServiceTestClass
    {
        private async Task SeedUser()
        {
            this.Db.Users.Add(new CakeItUser { UserName = "test@test.mail", EmailConfirmed = false, PhoneNumberConfirmed = false, TwoFactorEnabled = false, LockoutEnabled = false, AccessFailedCount = 0, CreatedOn = new DateTime(2018, 12, 14, 18, 22, 10, 20), DeletedOn = null, IsDeleted = false });

            await this.Db.SaveChangesAsync();
        }

        private async Task SeedShopigCart()
        {
            var repo = new Repository<ShoppingCartItem>(this.Db);

            var productRepo = new Repository<Product>(this.Db);

            var provider = new Mock<IServiceProvider>();

            var cart = new Mock<ShoppingCart>();

            var shoppingCartService = new ShoppingCartService(repo, productRepo, null, provider.Object, cart.Object);

            await shoppingCartService.AddToShoppingCart(1);
            await shoppingCartService.AddToShoppingCart(2);
        }

        [Fact]
        public async Task CreateOrder_ShouldReturnOrderId()
        {
            //Arrange
            await this.SeedProducts();
            await this.SeedUser();
            await this.SeedShopigCart();

            var repo = new Repository<Order>(this.Db);
            var userRepo = new Repository<CakeItUser>(this.Db);
            var cartItemRepo = new Repository<ShoppingCartItem>(this.Db);

            var item =  cartItemRepo.All().First();
            item.ShoppingCartId = "test@test.mail";
            await cartItemRepo.SaveChangesAsync();

            var orderService = new OrderService(repo, userRepo, cartItemRepo, this.Mapper);

            var userName = "test@test.mail";

            //Act
            var result = await orderService.CreateOrder(userName);

            //Asser
            Assert.Equal(1, result);
        }

        [Fact]
        public async Task CreateOrder_WithEmptyShoppingCart_ShouldThrow()
        {
            //Arrange
            await this.SeedProducts();
            await this.SeedUser();

            var repo = new Repository<Order>(this.Db);
            var userRepo = new Repository<CakeItUser>(this.Db);
            var cartItemRepo = new Repository<ShoppingCartItem>(this.Db);

            var orderService = new OrderService(repo, userRepo, cartItemRepo, this.Mapper);

            var userName = "test@test.mail";

            //Asser
           await Assert.ThrowsAsync<InvalidOperationException>(async () => await orderService.CreateOrder(userName));

        }

        [Fact]
        public async Task GetAllOrdersByUser_ShouldReturnCollectionOfOrders()
        {
            //Arrange
            await this.SeedProducts();
            await this.SeedUser();
            await this.SeedShopigCart();

            var repo = new Repository<Order>(this.Db);
            var userRepo = new Repository<CakeItUser>(this.Db);
            var cartItemRepo = new Repository<ShoppingCartItem>(this.Db);

            var item = cartItemRepo.All().First();
            item.ShoppingCartId = "test@test.mail";
            await cartItemRepo.SaveChangesAsync();

            var orderService = new OrderService(repo, userRepo, cartItemRepo, this.Mapper);

            var userName = "test@test.mail";

            await orderService.CreateOrder(userName);

            //Act
            var orders = orderService.GetAllOrdersByUser(userName);
            var expectedCount = 1;
            var ectualCount = orders.Count();

            //Asser
            Assert.Equal(expectedCount, ectualCount);
        }

        [Fact]
        public async Task GetAllOrdersByUser_WithNoOrders_ShouldReturnEmptyCollectionOfOrders()
        {
            //Arrange
            await this.SeedProducts();
            await this.SeedUser();

            var repo = new Repository<Order>(this.Db);
            var userRepo = new Repository<CakeItUser>(this.Db);
            var cartItemRepo = new Repository<ShoppingCartItem>(this.Db);

            var orderService = new OrderService(repo, userRepo, cartItemRepo, this.Mapper);

            var userName = "test@test.mail";


            //Act
            var orders = orderService.GetAllOrdersByUser(userName);
            var expectedCount = 0;
            var ectualCount = orders.Count();

            //Asser
            Assert.Equal(expectedCount, ectualCount);
        }

        [Fact]
        public async Task GetAllItemsPerOrder_ShouldReturnCollectionOfItems()
        {
            //Arrange
            await this.SeedProducts();
            await this.SeedUser();
            await this.SeedShopigCart();

            var repo = new Repository<Order>(this.Db);
            var userRepo = new Repository<CakeItUser>(this.Db);
            var cartItemRepo = new Repository<ShoppingCartItem>(this.Db);

            var userName = "test@test.mail";

            foreach (var item in cartItemRepo.All())
            {
                item.ShoppingCartId = userName;
            }

            await cartItemRepo.SaveChangesAsync();

            var orderService = new OrderService(repo, userRepo, cartItemRepo, this.Mapper);

            await orderService.CreateOrder(userName);

            //Act
            var result = orderService.GetAllItemsPerOrder(1);
            var expectedCount = 2
;
            var actualCount = result.Count();

            //Assert
            Assert.Equal(expectedCount, actualCount);
        }

        [Fact]
        public async Task AddOrderDetails_WithValidModel_ShouldReturnOrderDetailsId()
        {
            //Arrange
            var repo = new Repository<OrderDetails>(this.Db);
            var orderDetailsService = new OrderDetailsService(repo, this.Mapper);

            var model = new OrderDetailsViewModel
            {
                FirstName = "Test",
                LastName = "TestTest",
                Email = "test@test.mail",
                PhoneNumber = "+359/888777666",
                City = "Test",
                Country = "Test",
                Address ="Test test test 6"
            };

            //Act
            var id = await orderDetailsService.AddOrderDetails(model);

            //Assert
            Assert.Equal(1, id);
        }
    }
}
