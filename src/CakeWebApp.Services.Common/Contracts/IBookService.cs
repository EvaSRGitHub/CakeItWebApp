using CakeItWebApp.ViewModels.Books;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CakeWebApp.Services.Common.Contracts
{
    public interface IBookService
    {
         Task CreateBook(CreateBookViewModel model);

        IQueryable<BookIndexViewModel> GetAllBooks();

        Task AddRatingToBook(int bookId, double rating);

        Task<BookIndexViewModel> GetBookById(int id);

        Task UpdateBook(BookIndexViewModel model);

        Task DeleteBook(BookIndexViewModel model);
    }
}
