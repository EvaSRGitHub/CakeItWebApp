using CakeItWebApp.Data;
using CakeItWebApp.Models;
using CakeItWebApp.Services.Common.Repository;
using CakeItWebApp.ViewModels.Cakes;
using CakeItWebApp.ViewModels.Orders;
using CakeWebApp.Models;
using CakeWebApp.Services.Common.CommonServices;
using Microsoft.EntityFrameworkCore;
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
    public class OrderServiceTests:BaseServiceTestClass
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

        private async Task SeedUser(CakeItDbContext db)
        {
            db.Users.Add(new CakeItUser { UserName = "test@test.mail", EmailConfirmed = false, PhoneNumberConfirmed = false, TwoFactorEnabled = false, LockoutEnabled = false, AccessFailedCount = 0, CreatedOn = new DateTime(2018, 12, 14, 18, 22, 10, 20), DeletedOn = null, IsDeleted = false });

            await db.SaveChangesAsync();
        }

        private async Task SeedShopigCart(CakeItDbContext db)
        {
            var repo = new Repository<ShoppingCartItem>(db);

            var productRepo = new Repository<Product>(db);

            var provider = new Mock<IServiceProvider>();

            var cart = new Mock<ShoppingCart>();

            var shoppingCartService = new ShoppingCartService(repo, productRepo, null, provider.Object, cart.Object);

            await shoppingCartService.AddToShoppingCart(1);
            await shoppingCartService.AddToShoppingCart(2);
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
        public async Task CreateOrder_ShouldReturnOrderId()
        {
            //Arrange
            var db = this.SetDb();

            await this.SeedProducts(db);
            await this.SeedUser(db);
            await this.SeedShopigCart(db);

            var repo = new Repository<Order>(db);
            var userRepo = new Repository<CakeItUser>(db);
            var cartItemRepo = new Repository<ShoppingCartItem>(db);

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
            var db = this.SetDb();

            await this.SeedProducts(db);
            await this.SeedUser(db);

            var repo = new Repository<Order>(db);
            var userRepo = new Repository<CakeItUser>(db);
            var cartItemRepo = new Repository<ShoppingCartItem>(db);

            var orderService = new OrderService(repo, userRepo, cartItemRepo, this.Mapper);

            var userName = "test@test.mail";

            //Asser
           await Assert.ThrowsAsync<InvalidOperationException>(async () => await orderService.CreateOrder(userName));

        }

        [Fact]
        public async Task GetAllOrdersByUser_ShouldReturnCollectionOfOrders()
        {
            //Arrange
            var db = this.SetDb();

            await this.SeedProducts(db);
            await this.SeedUser(db);
            await this.SeedShopigCart(db);

            var repo = new Repository<Order>(db);
            var userRepo = new Repository<CakeItUser>(db);
            var cartItemRepo = new Repository<ShoppingCartItem>(db);


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
            var db = this.SetDb();

            await this.SeedProducts(db);
            await this.SeedUser(db);

            var repo = new Repository<Order>(db);
            var userRepo = new Repository<CakeItUser>(db);
            var cartItemRepo = new Repository<ShoppingCartItem>(db);

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
            var db = this.SetDb();

            await this.SeedProducts(db);
            await this.SeedUser(db);
            await this.SeedShopigCart(db);

            var repo = new Repository<Order>(db);
            var userRepo = new Repository<CakeItUser>(db);
            var cartItemRepo = new Repository<ShoppingCartItem>(db);

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
            var db = this.SetDb();

            var repo = new Repository<OrderDetails>(db);
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

        [Fact]
        public async Task SetOrderDetailsId_WithValidId_ShouldSetOrderDetailsToOrder()
        {
            //Arrange
            var db = this.SetDb();

            await this.SeedProducts(db);
            await this.SeedUser(db);
            await this.SeedShopigCart(db);

            var repo = new Repository<Order>(db);
            var userRepo = new Repository<CakeItUser>(db);
            var cartItemRepo = new Repository<ShoppingCartItem>(db);

            var item = cartItemRepo.All().First();
            item.ShoppingCartId = "test@test.mail";
            await cartItemRepo.SaveChangesAsync();

            var orderService = new OrderService(repo, userRepo, cartItemRepo, this.Mapper);

            var userName = "test@test.mail";

            var result = await orderService.CreateOrder(userName);

            var detailsRepo = new Repository<OrderDetails>(db);
            var orderDetailsService = new OrderDetailsService(detailsRepo, this.Mapper);

            var model = new OrderDetailsViewModel
            {
                FirstName = "Test",
                LastName = "TestTest",
                Email = "test@test.mail",
                PhoneNumber = "+359/888777666",
                City = "Test",
                Country = "Test",
                Address = "Test test test 6"
            };

            var id = await orderDetailsService.AddOrderDetails(model);

            //Act
            await orderService.SetOrderDetailsId(id);

            var expectedOrderDetailsId = 1;
            var actualOrerDetailsId = (await repo.GetByIdAsync(1)).OrderDetailsId;

            //Assert
            Assert.Equal(expectedOrderDetailsId, actualOrerDetailsId);
        }
    }
}
