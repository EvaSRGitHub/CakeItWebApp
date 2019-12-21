using CakeItWebApp.ViewModels.Books;
using CakeWebApp.Services.Common.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using X.PagedList;

namespace CakeItWebApp.Controllers
{
    [Authorize]
    public class BooksController : Controller
    {
        private const int MaxBooksPerPage = 3;

        private readonly IBookService bookService;
        private readonly IErrorService errorService;

        public BooksController(IBookService bookService, IErrorService errorService)
        {
            this.bookService = bookService;
            this.errorService = errorService;
        }

        public IActionResult Index(int? page)
        {
            var books = this.bookService.GetAllBooks();

            var nextPage = page ?? 1;

            var booksPerPage = books.ToPagedList(nextPage, MaxBooksPerPage);

            return View(booksPerPage);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Rate(int bookId, int rating)
        {
            try
            {
                await this.bookService.AddRatingToBook(bookId, rating);
            }
            catch (Exception e)
            {
                ViewData["Еrrors"] = e.Message;

                return this.View("Error");
            }

            TempData["Rate"] = "Your rating has been successfully registered.";

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public IActionResult CreateBook()
        {
            return this.View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateBook(CreateBookViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = this.ModelState.Values.SelectMany(p => p.Errors).Select(e => e.ErrorMessage).ToList();

                var errorModel = this.errorService.GetErrorModel(errors);

                return View("Error", errorModel);
            }

            try
            {
                await this.bookService.CreateBook(model);
            }
            catch (Exception e)
            {
                ViewData["Errors"] = e.Message;

                return this.View("Error");
            }

            return this.RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditBook(int id)
        {
            BookIndexViewModel model;

            try
            {
                model = await this.bookService.GetBookById(id);
            }
            catch (Exception e)
            {
                ViewData["Errors"] = e.Message;

                return this.View("Error");
            }

            return this.View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> EditBook(BookIndexViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = this.ModelState.Values.SelectMany(p => p.Errors).Select(e => e.ErrorMessage).ToList();

                var errorModel = this.errorService.GetErrorModel(errors);

                return View("Error", errorModel);
            }

            try
            {
                await this.bookService.UpdateBook(model);
            }
            catch (Exception e)
            {
                ViewData["Errors"] = e.Message;

                return this.View("Error");
            }

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            BookIndexViewModel model;

            try
            {
                model = await this.bookService.GetBookById(id);
            }
            catch (Exception e)
            {
                ViewData["Errors"] = e.Message;

                return this.View("Error");
            }

            return this.View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> DeleteBook(BookIndexViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = this.ModelState.Values.SelectMany(p => p.Errors).Select(e => e.ErrorMessage).ToList();

                var errorModel = this.errorService.GetErrorModel(errors);

                return View("Error", errorModel);
            }

            try
            {
                await this.bookService.DeleteBook(model);
            }
            catch (Exception e)
            {
                ViewData["Errors"] = e.Message;

                return this.View("Error");
            }

            return RedirectToAction("Index");
        }

        [Authorize]
         public async Task<FileStreamResult> Download(int id)
        {
            var book = await this.bookService.GetBookById(id);

            var downLoadUrl = book.DownloadUrl;

            HttpClient  client = new HttpClient();

             var stream = await client.GetStreamAsync(downLoadUrl);

            return new FileStreamResult(stream, new MediaTypeHeaderValue("application/pdf"))
            {
                FileDownloadName = book.Title + ".pdf"
            };
        }

        [Authorize(Roles = "Admin")]
        public IActionResult AllBooks()
        {
            var model = this.bookService.GetAllBooks().ToList();

            return this.View(model);
        }
    }
}
