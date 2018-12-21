using CakeItWebApp.Models;
using CakeItWebApp.Services.Common.Repository;
using CakeItWebApp.ViewModels.Tutorials;
using CakeWebApp.Services.Common.CommonServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CakeItWebApp.Services.Common.Tests
{
    public class TutorialServiceTests:BaseServiceTestClass
    {
        private async Task SeedTutorials()
        {
            var repo = new Repository<Tutorial>(this.Db);
            repo.Add(new Tutorial {Title = "Lotus Flower", Description="Lotus Test Descripion", Url="https://someurl.bg" });

            repo.Add(new Tutorial { Title = "Silver leaf", Description = "Silver leaf" +
                " Test Descripion", Url = "https://someurl2.bg" });

            await repo.SaveChangesAsync();
        }

        [Fact]
        public async Task AddTutorial_WithValidModel_ShouldBeSuccessful()
        {
            //Arrange
            var repo = new Repository<Tutorial>(this.Db);
            var service = new TutorialService(repo, this.Mapper);

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
            var repo = new Repository<Tutorial>(this.Db);
            var service = new TutorialService(repo, this.Mapper);

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
            var repo = new Repository<Tutorial>(this.Db);
            var service = new TutorialService(repo, this.Mapper);

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
            await this.SeedTutorials();
            var repo = new Repository<Tutorial>(this.Db);
            var service = new TutorialService(repo, this.Mapper);

            var tutorial = await repo.GetByIdAsync(2);
            this.Db.Entry(tutorial).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
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
            await this.SeedTutorials();
            var repo = new Repository<Tutorial>(this.Db);
            var service = new TutorialService(repo, this.Mapper);

            var tutorial = await repo.GetByIdAsync(2);
            this.Db.Entry(tutorial).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
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
            await this.SeedTutorials();
            var repo = new Repository<Tutorial>(this.Db);
            var service = new TutorialService(repo, this.Mapper);

            var tutorial = await repo.GetByIdAsync(2);
            this.Db.Entry(tutorial).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
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
            await this.SeedTutorials();
            var repo = new Repository<Tutorial>(this.Db);
            var service = new TutorialService(repo, this.Mapper);

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
            await this.SeedTutorials();
            var repo = new Repository<Tutorial>(this.Db);
            var service = new TutorialService(repo, this.Mapper);

            //Act

            //Asser
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await service.DeleteTutorial(3));
        }

        [Fact]
        public async Task GetTutorialById_WithValidId_ShouldReturnModel()
        {
            //Arrange
            await this.SeedTutorials();
            var repo = new Repository<Tutorial>(this.Db);
            var service = new TutorialService(repo, this.Mapper);

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
            await this.SeedTutorials();
            var repo = new Repository<Tutorial>(this.Db);
            var service = new TutorialService(repo, this.Mapper);

            //Act

            //Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await service.GetTutorialById(3));
        }

        [Fact]
        public async Task AddRatingToTutorial_WithValidData_ShouldAddRatingToTutorial()
        {
            //Arrange
            await this.SeedTutorials();
            var repo = new Repository<Tutorial>(this.Db);
            var service = new TutorialService(repo, this.Mapper);

            //Act
            await service.AddRatingToTutorial(2, 5);
            await repo.SaveChangesAsync();
            var expectedRate = 5;
            var actualRate = repo.All().SingleOrDefault(p => p.Id == 2).Rating;

            //Assert
            Assert.Equal(expectedRate, actualRate);
        }

        [Fact]
        public async Task AddRatingToTutorial_WithValidData_ShoulChangeRatingVotesOfTheTutorial()
        {
            //Arrange
            await this.SeedTutorials();
            var repo = new Repository<Tutorial>(this.Db);
            var service = new TutorialService(repo, this.Mapper);

            //Act
            await service.AddRatingToTutorial(2, 5);
            await repo.SaveChangesAsync();
            var expectedRatingVote = 1;
            var actualRatingVote = repo.All().SingleOrDefault(p => p.Id == 2).RatingVotes;

            //Assert
            Assert.Equal(expectedRatingVote, actualRatingVote);
        }

        [Fact]
        public async Task AddRatingToTutorial_WithAddingRate_ShoulIncreaseRating()
        {
            //Arrange
            await this.SeedTutorials();
            var repo = new Repository<Tutorial>(this.Db);
            var service = new TutorialService(repo, this.Mapper);

            //Act
            await service.AddRatingToTutorial(2, 5);
            await service.AddRatingToTutorial(2, 3);
            await repo.SaveChangesAsync();
            var expectedRate = 8;
            var actualRate = repo.All().SingleOrDefault(p => p.Id == 2).Rating;

            //Assert
            Assert.Equal(expectedRate, actualRate);
        }

        [Fact]
        public async Task AddRatingToTutorial_WithAddingRate_ShoulIncreaseRatingVotes()
        {
            //Arrange
            await this.SeedTutorials();
            var repo = new Repository<Tutorial>(this.Db);
            var service = new TutorialService(repo, this.Mapper);

            //Act
            await service.AddRatingToTutorial(2, 5);
            await service.AddRatingToTutorial(2, 3);
            await repo.SaveChangesAsync();
            var expectedRatingVotes = 2;
            var actualRatingVotes = repo.All().SingleOrDefault(p => p.Id == 2).RatingVotes;

            //Assert
            Assert.Equal(expectedRatingVotes, actualRatingVotes);
        }

        [Fact]
        public async Task AddRatingToTutorial_WithInValidCakeId_ShoulThrow()
        {
            //Arrange
            await this.SeedTutorials();
            var repo = new Repository<Tutorial>(this.Db);
            var service = new TutorialService(repo, this.Mapper);

            //Act

            //Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await service.AddRatingToTutorial(3, 5));
        }

        [Fact]
        public async Task AddRatingToTutorial_WithInValidRate_ShoulThrow()
        {
            //Arrange
            await this.SeedTutorials();
            var repo = new Repository<Tutorial>(this.Db);
            var service = new TutorialService(repo, this.Mapper);

            //Act

            //Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await service.AddRatingToTutorial(3, -5));
        }

        [Fact]
        public async Task GetTutorials_WithAddedTutorialsInTheDb_ShouldReturnCollectionOfTurorials()
        {
            //Arrange
            await this.SeedTutorials();
            var repo = new Repository<Tutorial>(this.Db);
            var service = new TutorialService(repo, this.Mapper);

            //Act
            var tutorials = service.GetTutorials();

            //Assert
            Assert.NotEmpty(tutorials);
        }

        [Fact]
        public void GetTutorials_WithNoTutorialsInTheDb_ShouldReturnEmpty()
        {
            //Arrange
            var repo = new Repository<Tutorial>(this.Db);
            var service = new TutorialService(repo, this.Mapper);

            //Act
            var tutorials = service.GetTutorials();

            //Assert
            Assert.Empty(tutorials);
        }
    }
}
