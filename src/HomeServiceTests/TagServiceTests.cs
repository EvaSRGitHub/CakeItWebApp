using CakeItWebApp.Data;
using CakeItWebApp.Models;
using CakeItWebApp.Services.Common.Repository;
using CakeItWebApp.ViewModels.Tags;
using CakeWebApp.Services.Common.CommonServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Xunit;

namespace CakeItWebApp.Services.Common.Tests
{
    public class TagServiceTests:BaseServiceTestClass
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

        private async Task SeedTag(CakeItDbContext db)
        {
            var repo = new Repository<Tag>(db);

            var service = new TagService(null, repo, this.Mapper);

            var modelA = new TagInputViewModel
            {
                Name = "Cakes",
            };

            await service.CreateTag(modelA);

            var modelB = new TagInputViewModel
            {
                Name = "Baking"
            };

            await service.CreateTag
(modelB);
        }

        [Fact]
        public async Task CreateTag_WithValidModel_ShouldSaveTagToDb()
        {
            //Arrange
            var db = this.SetDb();
            var repo = new Repository<Tag>(db);

            var service = new TagService(null, repo, this.Mapper);

            var modelA = new TagInputViewModel
            {
                Name = "Cakes",
            };

            //Act
            await service.CreateTag(modelA);

            //Assert
            Assert.NotEmpty(repo.All());

        }

        [Fact]
        public async Task CreateTag_WithDuplicateName_ShouldThrow()
        {
            //Arrange
            var db = this.SetDb();

            await this.SeedTag(db);

            var repo = new Repository<Tag>(db);

            var service = new TagService(null, repo, this.Mapper);

            var modelC = new TagInputViewModel
            {
                Name = "Cakes",
            };

            //Act

            //Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await service.CreateTag(modelC));

        }

        [Fact]
        public async Task UpdateTag_WithValidModel_ShouldUpdateTag()
        {
            //Arrange
            var db = this.SetDb();

            await this.SeedTag(db);

            var repo = new Repository<Tag>(db);

            var service = new TagService(null, repo, this.Mapper);

            var tag = await repo.GetByIdAsync(1);

            db.Entry(tag).State = EntityState.Detached;

            var model = this.Mapper.Map<TagInputViewModel>(tag);

            model.Name = "Cake";

            //Act
            await service.UpdateTag(model);

            var acualName = (await repo.GetByIdAsync(1)).Name;

            //Assert
            Assert.Equal("Cake", acualName);

        }

        [Fact]
        public async Task UpdateTag_WithDuplicateName_ShouldThrow()
        {
            //Arrange
            var db = this.SetDb();

            await this.SeedTag(db);

            var repo = new Repository<Tag>(db);

            var service = new TagService(null, repo, this.Mapper);

            var tag = await repo.GetByIdAsync(1);

            db.Entry(tag).State = EntityState.Detached;

            var model = this.Mapper.Map<TagInputViewModel>(tag);

            model.Name = "Baking";

            //Act

            //Assert
           await Assert.ThrowsAsync<InvalidOperationException>(async () => await service.UpdateTag(model));
        }

        [Fact]
        public async Task GetTagById_WithValidId_ShouldReturnTag()
        {
            //Arrange
            var db = this.SetDb();

            await this.SeedTag(db);

            var repo = new Repository<Tag>(db);

            var service = new TagService(null, repo, this.Mapper);

            //Act
            var result = await service.GetTagById(1);

            //Assert
            Assert.Equal(1, result.Id);
        }

        [Fact]
        public async Task GetTagById_WithInValidId_ShouldThrow()
        {
            //Arrange
            var db = this.SetDb();

            await this.SeedTag(db);

            var repo = new Repository<Tag>(db);

            var service = new TagService(null, repo, this.Mapper);

            //Act

            //Assert
           await Assert.ThrowsAsync<NullReferenceException>(async () => await service.GetTagById(3));
        }

        [Fact]
        public async Task GetAllTags_ShouldReturnTagCollection()
        {
            //Arrange
            var db = this.SetDb();

            await this.SeedTag(db);

            var repo = new Repository<Tag>(db);

            var service = new TagService(null, repo, this.Mapper);

            //Act
            var result = service.GetAllTags();

            //Assert
            Assert.NotNull(result);
        }
    }
}
