using AutoMapper;
using CakeItWebApp.Models;
using CakeItWebApp.Services.Common.Repository;
using CakeItWebApp.ViewModels.Books;
using CakeWebApp.Services.Common.Contracts;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CakeWebApp.Services.Common.CommonServices
{
    public class BookService:IBookService
    {
        private readonly IRepository<Book> bookRepo;
        private readonly IMapper mapper;
        private readonly ILogger<BookService> logger;

        public BookService(IRepository<Book> bookRepo, IMapper mapper, ILogger<BookService> logger)
        {
            this.bookRepo = bookRepo;
            this.mapper = mapper;
            this.logger = logger;
        }

        public async Task AddRatingToBook(int bookId, double rating)
        {
            var book = this.bookRepo.All().FirstOrDefault(b => b.Id == bookId);

            if (book == null)
            {
                throw new NullReferenceException("Book not found.");
            }

            if (rating < 1 && rating > 5)
            {
                throw new InvalidOperationException("Invlid rating value.");
            }

            book.Rating = book.Rating + rating ?? rating;

            book.RatingVotes = book.RatingVotes + 1 ?? 1;

            try
            {
                await this.bookRepo.SaveChangesAsync();
            }
            catch (Exception e)
            {
                this.logger.LogError(e.Message);

                throw new InvalidOperationException("Sorry, an error occurred while trying to rate a book.");
            }
        }

        public async Task CreateBook(CreateBookViewModel model)
        {
            if(model == null)
            {
                throw new NullReferenceException("Invalid book.");
            }

            var book = this.mapper.Map<CreateBookViewModel, Book>(model);

            this.bookRepo.Add(book);

            try
            {
                await this.bookRepo.SaveChangesAsync();
            }
            catch (Exception e)
            {
                this.logger.LogError(e.Message);

                throw new InvalidOperationException("Sorry, an error occurred while trying to create book.");
            }
        }

        public IQueryable<BookIndexViewModel> GetAllBooks()
        {
            var books = this.bookRepo.All();

            var model = this.mapper.ProjectTo<BookIndexViewModel>(books);

            return model;
        }
    }
}
