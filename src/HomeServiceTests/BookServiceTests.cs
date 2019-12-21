using CakeItWebApp.Data;
using CakeItWebApp.Models;
using CakeItWebApp.Services.Common.Repository;
using CakeItWebApp.ViewModels.Books;
using CakeWebApp.Services.Common.CommonServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CakeItWebApp.Services.Common.Tests
{
    public class BookServiceTests:BaseServiceTestClass
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

        private async Task SeedBook(CakeItDbContext db)
        {
            var repo = new Repository<Book>(db);

            var service = new BookService(repo, this.Mapper, null);

            var modelA = new CreateBookViewModel
            {
                Author = "Ema Tor",
                Title = "Cook with pleasure",
                CoverUrl = "https://res.cloudinary.com/cakeit/image/upload/v1546032585/DairyFreeRecipes.jpg",
                Description = "This book offers you an array of delicious dairy-free delights.",
                DownloadUrl = "https://res.cloudinary.com/cakeit/image/upload/v1545921235/50_Dairy-Free_Recipes.pdf",
                Pages = 23
            };

            await service.CreateBook(modelA);

            var modelB = new CreateBookViewModel
            {
                Author = "Various",
                Title = "Holiday Baking Experimente",
                CoverUrl = "https://res.cloudinary.com/cakeit/image/upload/v1546028277/HBE.pdf",
                Description = "This book offers you an array of delicious dairy-free delights.",
                DownloadUrl = "https://res.cloudinary.com/cakeit/image/upload/v1546032518/HoliayBakingExperiment.jpg",
                Pages = 23
            };

            await service.CreateBook(modelB);
        }

        [Fact]
        public async Task CreateBook_WithValidModel_ShouldAddBookToDb()
        {
            //Assert
            var db = this.SetDb();

            var repo = new Repository<Book>(db);

            var service = new BookService(repo, this.Mapper, null);

            var model = new CreateBookViewModel
            {
                Author = "Ema Tor",
                Title = "Cook with pleasure",
                CoverUrl = "https://res.cloudinary.com/cakeit/image/upload/v1546032585/DairyFreeRecipes.jpg",
                Description = "This book offers you an array of delicious dairy-free delights.",
                DownloadUrl = "https://res.cloudinary.com/cakeit/image/upload/v1545921235/50_Dairy-Free_Recipes.pdf",
                Pages = 23
            };

            //Act
            await service.CreateBook(model);

            //Assert
            Assert.NotEmpty(repo.All());

        }

        [Fact]
        public async Task CreateBook_WithInValidModel_ShouldAddBookToDb()
        {
            //Assert
            var db = this.SetDb();

            var repo = new Repository<Book>(db);

            var service = new BookService(repo, this.Mapper, null);

            CreateBookViewModel model = null;

            //Act

            //Assert
            await Assert.ThrowsAsync<NullReferenceException>(async () => await service.CreateBook(model));

        }

        [Fact]
        public async Task DeleteBook_WithValidModel_ShouldReturnEmptyRepo()
        {
            //Assert
            var db = this.SetDb();

            await this.SeedBook(db);

            var repo = new Repository<Book>(db);

            var service = new BookService(repo, this.Mapper, null);

            var book = await repo.GetByIdAsync(1);

            db.Entry(book).State = EntityState.Detached;

            var model = this.Mapper.Map<BookIndexViewModel>(book);

            //Act
            await service.DeleteBook(model);

            //Assert
            Assert.Equal(1, await repo.All().CountAsync());
        }

        [Fact]
        public async Task DeleteBook_WithNullModel_ShouldReturnEmptyRepo()
        {
            //Assert
            var db = this.SetDb();

            await this.SeedBook(db);

            var repo = new Repository<Book>(db);

            var service = new BookService(repo, this.Mapper, null);

            BookIndexViewModel model = null;

            //Act

            //Assert
            await Assert.ThrowsAsync<NullReferenceException>(async () => await service.DeleteBook(model));
        }

        [Fact]
        public async Task AddRatingToBook_WithValidData_ShouldAddRating()
        {
            //Arrange
            var db = this.SetDb();

            await this.SeedBook(db);

            var repo = new Repository<Book>(db);

            var service = new BookService(repo, this.Mapper, null);

            //Act
            await service.AddRatingToBook(2, 5);

            var expectedRate = 5;
            var actualRate = repo.All().SingleOrDefault(p => p.Id == 2).Rating;

            //Assert
            Assert.Equal(expectedRate, actualRate);
        }

        [Fact]
        public async Task AddRatingToBook_WithValidData_ShoulChangeRatingVotes()
        {
            //Arrange
            var db = this.SetDb();

            await this.SeedBook(db);

            var repo = new Repository<Book>(db);

            var service = new BookService(repo, this.Mapper, null);

            //Act
            await service.AddRatingToBook(2, 5);

            var expectedRatingVote = 1;
            var actualRatingVote = repo.All().SingleOrDefault(p => p.Id == 2).RatingVotes;

            //Assert
            Assert.Equal(expectedRatingVote, actualRatingVote);
        }

        [Fact]
        public async Task AddRatingToBook_WithAddingRate_ShoulIncreaseRating()
        {
            //Arrange
            var db = this.SetDb();

            await this.SeedBook(db);

            var repo = new Repository<Book>(db);

            var service = new BookService(repo, this.Mapper, null);

            //Act
            await service.AddRatingToBook(2, 5);
            await service.AddRatingToBook(2, 3);

            var expectedRate = 8;
            var actualRate = repo.All().SingleOrDefault(p => p.Id == 2).Rating;

            //Assert
            Assert.Equal(expectedRate, actualRate);
        }

        [Fact]
        public async Task AddRatingToBook_WithAddingRate_ShoulIncreaseRatingVotes()
        {
            //Arrange
            var db = this.SetDb();

            await this.SeedBook(db);

            var repo = new Repository<Book>(db);

            var service = new BookService(repo, this.Mapper, null);

            //Act
            await service.AddRatingToBook(2, 5);
            await service.AddRatingToBook(2, 3);

            var expectedRatingVotes = 2;
            var actualRatingVotes = repo.All().SingleOrDefault(p => p.Id == 2).RatingVotes;

            //Assert
            Assert.Equal(expectedRatingVotes, actualRatingVotes);
        }

        [Fact]
        public async Task AddRatingToBook_WithInValidCakeId_ShoulThrow()
        {
            //Arrange
            var db = this.SetDb();

            await this.SeedBook(db);

            var repo = new Repository<Book>(db);

            var service = new BookService(repo, this.Mapper, null);

            //Act

            //Assert
            await Assert.ThrowsAsync<NullReferenceException>(async () => await service.AddRatingToBook(3, 5));
        }

        [Fact]
        public async Task AddRatingToBook_WithInValidRate_ShoulThrow()
        {
            //Arrange
            var db = this.SetDb();

            await this.SeedBook(db);

            var repo = new Repository<Book>(db);

            var service = new BookService(repo, this.Mapper, null);

            //Act

            //Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await service.AddRatingToBook(2, 6));
        }

        [Fact]
        public async Task UpdateBook_WithValidData_ShouldSaveUpdatedBookInDb()
        {
            //Arange
            var db = this.SetDb();

            await this.SeedBook(db);

            var repo = new Repository<Book>(db);

            var bookService = new BookService(repo, this.Mapper, null);

            var entity = repo.All().SingleOrDefault(p => p.Id == 1);

            db.Entry(entity).State = EntityState.Detached;

            entity.Title = "Cook with not pleasure";

            //Act
            await bookService.UpdateBook(this.Mapper.Map<BookIndexViewModel>(entity));

            var actual = (await repo.GetByIdAsync(1)).Title;

            var expected = "Cook with not pleasure";

            //Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task UpdateBook_WithNullData_ShouldThrow()
        {
            //Arange
            var db = this.SetDb();

            await this.SeedBook(db);

            var repo = new Repository<Book>(db);

            var bookService = new BookService(repo, this.Mapper, null);

            BookIndexViewModel model = null;

            //Act

            //Assert
            await Assert.ThrowsAsync<NullReferenceException>(async () => await bookService.UpdateBook(this.Mapper.Map<BookIndexViewModel>(model)));
        }

        [Fact]
        public async Task UpdateBook_WithdDuplicateTitle_ShouldNotUpdatedBook()
        {
            //Arange
            var db = this.SetDb();

            await this.SeedBook(db);

            var repo = new Repository<Book>(db);

            var bookService = new BookService(repo, this.Mapper, null);

            var entity = repo.All().SingleOrDefault(p => p.Id == 2);

            db.Entry(entity).State = EntityState.Detached;

            var model = this.Mapper.Map<BookIndexViewModel>(entity);
            model.Title = "Cook with pleasure";

            //Act

            //Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await bookService.UpdateBook(model));
        }

        [Fact]
        public async Task UpdateBook_WithdDuplicateDownloadUrl_ShouldNotUpdatedBook()
        {
            //Arange
            var db = this.SetDb();

            await this.SeedBook(db);

            var repo = new Repository<Book>(db);

            var bookService = new BookService(repo, this.Mapper, null);

            var entity = repo.All().SingleOrDefault(p => p.Id == 2);

            db.Entry(entity).State = EntityState.Detached;

            var model = this.Mapper.Map<BookIndexViewModel>(entity);
            model.DownloadUrl = "https://res.cloudinary.com/cakeit/image/upload/v1545921235/50_Dairy-Free_Recipes.pdf";

            //Act

            //Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await bookService.UpdateBook(model));
        }

        [Fact]
        public async Task GetAllBooks_ShouldReturn_CollectionOfBooks()
        {
            //Arange
            var db = this.SetDb();

            await this.SeedBook(db);

            var repo = new Repository<Book>(db);

            var bookService = new BookService(repo, this.Mapper, null);

            //Act
            var result = bookService.GetAllBooks();

            //Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetBookById_ShouldReturnBook()
        {
            //Arange
            var db = this.SetDb();

            await this.SeedBook(db);

            var repo = new Repository<Book>(db);

            var bookService = new BookService(repo, this.Mapper, null);

            //Act
            var result = await bookService.GetBookById(2);

            //Assert
            Assert.NotNull(result);
        }
    }
}
