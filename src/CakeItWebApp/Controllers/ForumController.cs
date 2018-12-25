using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CakeItWebApp.ViewModels.Forum;
using CakeWebApp.Services.Common.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CakeItWebApp.Controllers
{
    [Authorize]
    public class ForumController : Controller
    {
        private readonly IForumService forumService;
        private readonly IErrorService errorService;
        private readonly ITagService tagService;

        public ForumController(IForumService forumService, IErrorService errorService, ITagService tagService)
        {
            this.forumService = forumService;
            this.errorService = errorService;
            this.tagService = tagService;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            var tags = this.tagService.GetAllTags().ToList().Select( t => t.Name);

            ViewData["Tags"] = string.Join(", ", tags);

            var allPosts = this.forumService.GetAllPosts();

            return View(allPosts);

            //TODO: PAGING
        }

        public IActionResult CreatePost()
        {
            var tags = this.tagService.GetAllTags().ToList().Select(t => t.Name);
            ViewData["Tags"] = string.Join(", ", tags);

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreatePost(PostInputViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = this.ModelState.Values.SelectMany(p => p.Errors).Select(e => e.ErrorMessage).ToList();

                var errorModel = this.errorService.GetErrorModel(errors);

                return View("Error", errorModel);
            }

            try
            {
                await this.forumService.CreatePost(model);
            }
            catch (Exception e)
            {
                ViewData["Errors"] = e.Message;
                return this.View("Error");
            }

            return RedirectToAction("Index");
        }

        public IActionResult PostDetails(int id)
        {
            PostDetailsViewModel postDetail = null;

            try
            {
                postDetail = this.forumService.GetPostDetailById(id);
            }
            catch (Exception e)
            {
                ViewData["Errors"] = e.Message;

                return this.View("Error");
            }

            return this.View(postDetail);
        }

        [HttpPost]
        public async Task<IActionResult> Comment(PostDetailsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = this.ModelState.Values.SelectMany(p => p.Errors).Select(e => e.ErrorMessage).ToList();

                var errorModel = this.errorService.GetErrorModel(errors);

                return View("Error", errorModel);
            }

            var comment = model.CreateComment;

            try
            {
                await this.forumService.CreateComment(comment);
            }
            catch (Exception e)
            {
                ViewData["Errors"] = e.Message;

                return this.View("Error");
            }
            return RedirectToAction("PostDetails", model.Id);
        }

        public async Task<IActionResult> EditPost(int id)
        {
            var tags = this.tagService.GetAllTags().ToList().Select(t => t.Name);
            ViewData["Tags"] = string.Join(", ", tags);

            PostInputViewModel model;
            try
            {
                model = await this.forumService.GetPostById(id);
            }
            catch (Exception e)
            {
                ViewData["Errors"] = e.Message;

                return this.View("Error");
            }

            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditPost(PostInputViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = this.ModelState.Values.SelectMany(p => p.Errors).Select(e => e.ErrorMessage).ToList();

                var errorModel = this.errorService.GetErrorModel(errors);

                return View("Error", errorModel);
            }

            try
            {
                await this.forumService.UpdatePost(model);
            }
            catch (Exception e)
            {
                ViewData["Errors"] = e.Message;
                return this.View("Error");
            }

            return RedirectToAction("PostDetails", new { id = model.Id });
        }

       
        public async Task<IActionResult> SoftDeletePost(int id)
        {
            PostInputViewModel model;

            try
            {
                model = await this.forumService.GetPostById(id);
            }
            catch (Exception e)
            {
                ViewData["Errors"] = e.Message;

                return this.View("Error");
            }

            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SoftDeletePost(PostInputViewModel model)
        {
            try
            {
                await this.forumService.MarkPostAsDeleted(model);
            }
            catch (Exception e)
            {
                ViewData["Errors"] = e.Message;

                return this.View("Error");
            }

            return this.RedirectToAction("Index");
        }
        //Edit Comment

        //Delete Comment

        

        //TODO - All Delets
    }
}

