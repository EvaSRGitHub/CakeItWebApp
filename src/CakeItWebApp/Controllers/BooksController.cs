using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CakeItWebApp.ViewModels.Books;
using CakeWebApp.Services.Common.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;

namespace CakeItWebApp.Controllers
{
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
    }
}
