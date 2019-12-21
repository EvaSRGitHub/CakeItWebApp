using CakeItWebApp.Data;
using CakeItWebApp.Models;
using CakeItWebApp.Services.Common.Repository;
using CakeItWebApp.ViewModels.Tutorials;
using CakeWebApp.Services.Common.CommonServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CakeItWebApp.Services.Common.Tests
{
    public class TutorialServiceTests:BaseServiceTestClass
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

        private async Task SeedTutorials(CakeItDbContext db)
        {
            var repo = new Repository<Tutorial>(db);
            repo.Add(new Tutorial {Title = "Lotus Flower", Description="Lotus Test Descripion", Url="https://someurl.bg" });

            repo.Add(new Tutorial { Title = "Silver leaf", Description = "Silver leaf" +
                " Test Descripion", Url = "https://someurl2.bg" });

            await repo.SaveChangesAsync();
        }

        [Fact]
        public async Task AddTutorial_WithValidModel_ShouldBeSuccessful()
        {
            //Arrange
            var db = this.SetDb();

            var repo = new Repository<Tutorial>(db);

            var service = new TutorialService(repo, this.Mapper,null);

            var model = new AddTutorialViewModel { Title = "Lotus Flower", Description = "Lotus Test Descripion", Url = "https://someurl.bg" };

            //Act
            await service.AddTutorial(model);
            var expectedTutorialsCount = 1;
            var actualTutorialsCount = repo.All().Count();

            //Assert
            Assert.Equal(expectedTutorialsCount, actualTutorialsCount);
        }

        [Fact]
        public async Task AddTutorial_WithDuplicateUrl_ShouldTrow()
        {
            //Arrange
            var db = this.SetDb();

            var repo = new Repository<Tutorial>(db);

            var service = new TutorialService(repo, this.Mapper, null);

            var model1 = new AddTutorialViewModel { Title = "Lotus Flower", Description = "Lotus Test Descripion", Url = "https://someurl.bg" };
            var model2 = new AddTutorialViewModel { Title = "Flower", Description = "Lotus Test Descripion", Url = "https://someurl.bg" };

            await service.AddTutorial(model1);
            //Act

            //Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await service.AddTutorial(model2));
        }

        [Fact]
        public async Task AddTutorial_WithDuplicateTitle_ShouldTrow()
        {
            //Arrange
            var db = this.SetDb();

            var repo = new Repository<Tutorial>(db);

            var service = new TutorialService(repo, this.Mapper, null);

            var model1 = new AddTutorialViewModel { Title = "Lotus Flower", Description = "Lotus Test Descripion", Url = "https://someurl.bg" };
            var model2 = new AddTutorialViewModel { Title = "Lotus Flower", Description = "Lotus Test Descripion", Url = "https://otherurl.bg" };

            await service.AddTutorial(model1);
            //Act

            //Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await service.AddTutorial(model2));
        }

        [Fact]
        public async Task Edit_WithValidData_ShouldSaveUpdatedTurorial()
        {
            //Arrange
            var db = this.SetDb();

            await this.SeedTutorials(db);

            var repo = new Repository<Tutorial>(db);

            var service = new TutorialService(repo, this.Mapper, null);

            var tutorial = await repo.GetByIdAsync(2);
            db.Entry(tutorial).State = EntityState.Detached;
            var model = this.Mapper.Map<Tutorial, AddTutorialViewModel>(tutorial);
            model.Title = "Golden leaf";

            //Act
            await service.UpdateTutorial(model);
            
            var expectedTitle = "Golden leaf";
            var actualTitle = repo.All().SingleOrDefault(t => t.Id == 2).Title;

            //Assert
            Assert.Equal(expectedTitle, actualTitle);
        }

        [Fact]
        public async Task Edit_WithDuplicateTitle_ShouldSaveUpdatedTurorialThrow()
        {
            //Arrange
            var db = this.SetDb();

            await this.SeedTutorials(db);

            var repo = new Repository<Tutorial>(db);

            var service = new TutorialService(repo, this.Mapper, null);

            var tutorial = await repo.GetByIdAsync(2);
            db.Entry(tutorial).State = EntityState.Detached;
            var model = this.Mapper.Map<Tutorial, AddTutorialViewModel>(tutorial);
            model.Title = "Lotus Flower";

            //Act

            //Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await service.UpdateTutorial(model));
        }

        [Fact]
        public async Task Edit_WithDuplicateUrl_ShouldSaveUpdatedTurorialThrow()
        {
            //Arrange
            var db = this.SetDb();

            await this.SeedTutorials(db);

            var repo = new Repository<Tutorial>(db);

            var service = new TutorialService(repo, this.Mapper, null);

            var tutorial = await repo.GetByIdAsync(2);
            db.Entry(tutorial).State = EntityState.Detached;
            var model = this.Mapper.Map<Tutorial, AddTutorialViewModel>(tutorial);
            model.Url = "https:" +
                "//someurl.bg";

            //Act

            //Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await service.UpdateTutorial(model));
        }

        [Fact]
        public async Task DeleteTutorial_WithValidiD_ShouldDeleteTheTutorial()
        {
            //Arrange
            var db = this.SetDb();

            await this.SeedTutorials(db);

            var repo = new Repository<Tutorial>(db);

            var service = new TutorialService(repo, this.Mapper, null);

            //Act
            await service.DeleteTutorial(1);
            var expectedRepoCount = 1;
            var actualRepoCount = repo.All().Count();

            var expectedTutorialId = 2;
            var actualTutorialId = repo.All().First().Id;

            //Asser
            Assert.Equal(expectedRepoCount, actualRepoCount);
            Assert.Equal(expectedTutorialId, actualTutorialId);
        }

        [Fact]
        public async Task DeleteTutorial_WithInValidiD_ShouldDoNating()
        {
            //Arrange
            var db = this.SetDb();

            await this.SeedTutorials(db);

            var repo = new Repository<Tutorial>(db);

            var service = new TutorialService(repo, this.Mapper, null);

            //Act

            //Asser
            await Assert.ThrowsAsync<NullReferenceException>(async () => await service.DeleteTutorial(3));
        }

        [Fact]
        public async Task GetTutorialById_WithValidId_ShouldReturnModel()
        {
            //Arrange
            var db = this.SetDb();

            await this.SeedTutorials(db);

            var repo = new Repository<Tutorial>(db);

            var service = new TutorialService(repo, this.Mapper, null);

            //Act
            var model = await service.GetTutorialById(2);

            var expectedId = 2;
            var actualId = model.Id;

            //Assert
            Assert.NotNull(model);
            Assert.Equal(expectedId, actualId);
        }

        [Fact]
        public async Task GetTutorialById_WithInValidId_ShouldThrow()
        {
            //Arrange
            var db = this.SetDb();

            await this.SeedTutorials(db);

            var repo = new Repository<Tutorial>(db);

            var service = new TutorialService(repo, this.Mapper, null);

            //Act

            //Assert
            await Assert.ThrowsAsync<NullReferenceException>(async () => await service.GetTutorialById(3));
        }

        [Fact]
        public async Task AddRatingToTutorial_WithValidData_ShouldAddRatingToTutorial()
        {
            //Arrange
            var db = this.SetDb();

            await this.SeedTutorials(db);

            var repo = new Repository<Tutorial>(db);
            var service = new TutorialService(repo, this.Mapper, null);

            //Act
            await service.AddRatingToTutorial(2, 5);

            var expectedRate = 5;
            var actualRate = repo.All().SingleOrDefault(p => p.Id == 2).Rating;

            //Assert
            Assert.Equal(expectedRate, actualRate);
        }

        [Fact]
        public async Task AddRatingToTutorial_WithInValidRating_ShouldThrow()
        {
            //Arrange
            var db = this.SetDb();

            await this.SeedTutorials(db);

            var repo = new Repository<Tutorial>(db);
            var service = new TutorialService(repo, this.Mapper, null);

            //Act

            //Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await service.AddRatingToTutorial(2, 6));
        }

        [Fact]
        public async Task AddRatingToTutorial_WithValidData_ShoulChangeRatingVotesOfTheTutorial()
        {
            //Arrange
            var db = this.SetDb();

            await this.SeedTutorials(db);

            var repo = new Repository<Tutorial>(db);

            var service = new TutorialService(repo, this.Mapper, null);

            //Act
            await service.AddRatingToTutorial(2, 5);

            var expectedRatingVote = 1;
            var actualRatingVote = repo.All().SingleOrDefault(p => p.Id == 2).RatingVotes;

            //Assert
            Assert.Equal(expectedRatingVote, actualRatingVote);
        }

        [Fact]
        public async Task AddRatingToTutorial_WithAddingRate_ShoulIncreaseRating()
        {
            //Arrange
            var db = this.SetDb();

            await this.SeedTutorials(db);

            var repo = new Repository<Tutorial>(db);

            var service = new TutorialService(repo, this.Mapper, null);

            //Act
            await service.AddRatingToTutorial(2, 5);
            await service.AddRatingToTutorial(2, 3);
           
            var expectedRate = 8;
            var actualRate = repo.All().SingleOrDefault(p => p.Id == 2).Rating;

            //Assert
            Assert.Equal(expectedRate, actualRate);
        }

        [Fact]
        public async Task AddRatingToTutorial_WithAddingRate_ShoulIncreaseRatingVotes()
        {
            //Arrange
            var db = this.SetDb();

            await this.SeedTutorials(db);

            var repo = new Repository<Tutorial>(db);

            var service = new TutorialService(repo, this.Mapper, null);

            //Act
            await service.AddRatingToTutorial(2, 5);
            await service.AddRatingToTutorial(2, 3);
           
            var expectedRatingVotes = 2;
            var actualRatingVotes = repo.All().SingleOrDefault(p => p.Id == 2).RatingVotes;

            //Assert
            Assert.Equal(expectedRatingVotes, actualRatingVotes);
        }

        [Fact]
        public async Task AddRatingToTutorial_WithInValidCakeId_ShoulThrow()
        {
            //Arrange
            var db = this.SetDb();

            await this.SeedTutorials(db);

            var repo = new Repository<Tutorial>(db);

            var service = new TutorialService(repo, this.Mapper, null);

            //Act

            //Assert
            await Assert.ThrowsAsync<NullReferenceException>(async () => await service.AddRatingToTutorial(3, 5));
        }

        [Fact]
        public async Task AddRatingToTutorial_WithNullTutorial_ShoulThrow()
        {
            //Arrange
            var db = this.SetDb();

            await this.SeedTutorials(db);

            var repo = new Repository<Tutorial>(db);

            var service = new TutorialService(repo, this.Mapper, null);

            //Act

            //Assert
            await Assert.ThrowsAsync<NullReferenceException>(async () => await service.AddRatingToTutorial(3, -5));
        }

        [Fact]
        public async Task GetTutorials_WithAddedTutorialsInTheDb_ShouldReturnCollectionOfTurorials()
        {
            //Arrange
            var db = this.SetDb();

            await this.SeedTutorials(db);

            var repo = new Repository<Tutorial>(db);

            var service = new TutorialService(repo, this.Mapper, null);

            //Act
            var tutorials = service.GetTutorials();

            //Assert
            Assert.NotEmpty(tutorials);
        }

        [Fact]
        public void GetTutorials_WithNoTutorialsInTheDb_ShouldReturnEmpty()
        {
            //Arrange
            var db = this.SetDb();

            var repo = new Repository<Tutorial>(db);

            var service = new TutorialService(repo, this.Mapper, null);

            //Act
            var tutorials = service.GetTutorials();

            //Assert
            Assert.Empty(tutorials);
        }
    }
}
