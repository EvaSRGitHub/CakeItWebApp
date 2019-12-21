using CakeItWebApp.ViewModels.Books;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CakeWebApp.Services.Common.Contracts
{
    public interface IBookService
    {
         Task CreateBook(CreateBookViewModel model);

        IEnumerable<BookIndexViewModel> GetAllBooks();

        Task AddRatingToBook(int bookId, double rating);

        Task<BookIndexViewModel> GetBookById(int id);

        Task UpdateBook(BookIndexViewModel model);

        Task DeleteBook(BookIndexViewModel model);
    }
}
