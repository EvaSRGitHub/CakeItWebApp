using AutoMapper;
using CakeItWebApp.Models;
using CakeItWebApp.Services.Common.Repository;
using CakeItWebApp.ViewModels.Forum;
using CakeItWebApp.ViewModels.Tags;
using CakeWebApp.Services.Common.Contracts;
using CakeWebApp.Services.Common.Sanitizer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace CakeWebApp.Services.Common.CommonServices
{
    public class ForumService : IForumService
    {
        private readonly ILogger<ForumService> logger;
        private readonly IRepository<Post> postRepo;
        private readonly IRepository<Comment> commentRepo;
        private readonly IRepository<CakeItUser> userRepo;
        private readonly IRepository<Tag> tagRepo;
        private readonly IRepository<TagPosts> tagPostsRepo;
        private readonly IMapper mapper;
        private readonly ISanitizer sanitizer;

        public ForumService(ILogger<ForumService> logger, IRepository<Post> postRepo, IRepository<Comment> commentRepo, IRepository<CakeItUser> userRepo, IRepository<Tag> tagRepo, IRepository<TagPosts> tagPostsRepo, IMapper mapper, ISanitizer sanitizer)
        {
            this.logger = logger;
            this.postRepo = postRepo;
            this.commentRepo = commentRepo;
            this.userRepo = userRepo;
            this.tagRepo = tagRepo;
            this.tagPostsRepo = tagPostsRepo;
            this.mapper = mapper;
            this.sanitizer = sanitizer;
        }

        public async Task CreateComment(CommentInputViewModel model)
        {
            if (model == null)
            {
                throw new NullReferenceException("Sorry, couldn't process your comment.");
            }

            var content = model.Content;

            model.AuthorId = this.userRepo.All().SingleOrDefault(u => u.UserName == model.AuthorName).Id;

            var comment = this.mapper.Map<CommentInputViewModel, Comment>(model);

            this.commentRepo.Add(comment);

            try
            {
                await this.commentRepo.SaveChangesAsync();
            }
            catch (Exception e)
            {
                this.logger.LogDebug(e.Message);

                throw new InvalidOperationException("Sorry, couldn't process your comment.");
            }
        }

        public async Task CreatePost(PostInputViewModel model)
        {
            var authorId = this.userRepo.All().SingleOrDefault(u => u.UserName == model.Author).Id;

            var content = model.FullContent.Trim();

            var title = model.Title.Trim();

            if (string.IsNullOrWhiteSpace(title))
            {
                throw new NullReferenceException("Title is required.");
            }

            var inputTags = model.Tags.Trim();

            if (string.IsNullOrWhiteSpace(inputTags))
            {
                throw new NullReferenceException("Tag is required.");
            }

            var dataTags = this.tagPostsRepo.All().AsNoTracking().Select(t => t.Tag.Name).ToList();

            var tags = model.Tags.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var tag in tags)
            {
                if(!dataTags.Any(t => t.Equals(tag, StringComparison.InvariantCultureIgnoreCase)))
                {
                    throw new InvalidOperationException($"Invalid tag {tag}");
                }
            }

            var post = new Post
            {
                AuthorId = authorId,
                CreatedOn = model.CreatedOn,
                FullContent = model.FullContent,
                Title = model.Title,
            };

            if (postRepo.All().Any(p => p.Title == post.Title && p.IsDeleted == false))
            {
                throw new InvalidOperationException("Post with such title already exists.");
            }

            this.postRepo.Add(post);

            try
            {
                await this.postRepo.SaveChangesAsync();

                foreach (var tag in tags)
                {
                    var tagId = tagRepo.All().SingleOrDefault(t => t.Name == tag).Id;

                    var tagPosts = new TagPosts
                    {
                        TagId = tagId,
                        PostId = post.Id
                    };

                    post.Tags.Add(tagPosts);
                }

                await this.postRepo.SaveChangesAsync();

            }
            catch (Exception e)
            {
                this.logger.LogDebug(e.Message);

                throw new InvalidOperationException("Sorry. An error occurred while creating a post.");
            }
        }

        public async Task<PostDetailsViewModel> GetAllCommentsPerPost(int postId)
        {
            var post = await this.postRepo.GetByIdAsync(postId);

            if (post == null && post.IsDeleted == true)
            {
                return null;
            }

            var postDetail = new PostDetailsViewModel
            {
                Author = post.Author.UserName,
                Title = this.sanitizer.Sanitize(post.Title),
                FullContent = this.sanitizer.Sanitize(post.FullContent),
                CreatedOn = post.CreatedOn.ToString("dd-MM-yyyy HH:mm"),
                Tags = string.Join(", ", post.Tags.Select(t => t.Tag.Name)),
                Id = post.Id,
                IsDeleted = post.IsDeleted,
                Comments = post.Comments.Where(c => c.IsDeleted == false).Select(c => new CommentViewModel
                {
                    AuthorName = c.Author.UserName,
                    Content = this.sanitizer.Sanitize(c.Content),
                    CreatedOn = c.CreatedOn.ToString("dd-MM-yyyy HH:mm"),
                    Id = c.Id,
                    IsDeleted = c.IsDeleted
                }).OrderByDescending(c => c.CreatedOn).ToList()
            };

            return postDetail;
        }

        public IQueryable<CommentInputViewModel> GetAllMyComments(string userName)
        {
            var comments = this.commentRepo.All().Where(a => a.Author.UserName == userName && a.IsDeleted == false);

            if (comments.Count() == 0)
            {
                return null;
            }

            var commentModels = comments.Select(c => new CommentInputViewModel
            {
                CreatedOn = c.CreatedOn,
                Content = GetShortContent(c.Content),
                AuthorId = c.AuthorId,
                Id = c.Id,
                PostId = c.PostId,
                IsDeleted = c.IsDeleted
            }).OrderByDescending(c => c.CreatedOn.Date);

            return commentModels;
        }

        public IQueryable<UserPostsViewModel> GetAllMyPosts(string userName)
        {
            var allPosts = this.postRepo.All().AsNoTracking().Where(u => u.Author.UserName == userName && u.IsDeleted == false);

            if (allPosts.Count() == 0)
            {
                return null;
            }

            var modelPosts = allPosts.Select(p => new UserPostsViewModel
            {
                PostId = p.Id,
                Title = this.sanitizer.Sanitize(p.Title),
                CreatedOn = p.CreatedOn.ToString("dd-MM-yyyy HH:mm"),
                Content = GetShortContent(p.FullContent),
                CommentsCount = p.Comments.Where(c => c.IsDeleted == false).Count()
            }).OrderByDescending(p => p.CreatedOn);

            return modelPosts;
        }

        public IEnumerable<PostIndexViewModel> GetAllActivePosts(string searchString)
        {
            IQueryable<Post> posts;

            if (searchString == null)
            {
                posts = this.postRepo.All().AsNoTracking().Where(p => p.IsDeleted == false);
            }
            else
            {
                posts = this.postRepo.All().AsNoTracking().Where(p => p.Title.Contains(searchString, StringComparison.CurrentCultureIgnoreCase) && p.IsDeleted == false);
            }

          var modelPosts = this.mapper.ProjectTo<PostIndexViewModel>(posts).OrderByDescending(p => p.CreatedOn).ToList();

            return modelPosts;
        }

        public async Task<EditCommentViewModel> GetCommentToEditOrDelete(int id)
        {
            var commentToEdit = await this.commentRepo.GetByIdAsync(id);

            if (commentToEdit == null)
            {
                throw new NullReferenceException("Comment not found.");
            }

            if (commentToEdit.Post == null || commentToEdit.Post.IsDeleted == true)
            {
                throw new InvalidOperationException("Post not found.");
            }

            var model = this.mapper.Map<Comment, EditCommentViewModel>(commentToEdit);

            model.FullContent = this.sanitizer.Sanitize(model.FullContent);

            model.Content = this.sanitizer.Sanitize(model.Content);

            return model;
        }

        public async Task<PostInputViewModel> GetPostById(int id)
        {
            var post = await this.postRepo.GetByIdAsync(id);

            if (post == null)
            {
                throw new NullReferenceException("Post not found");
            }

            if (post.IsDeleted)
            {
                throw new InvalidOperationException("Post not found");
            }

            var postModel = new PostInputViewModel
            {
                Author = post.Author.UserName,
                Id = post.Id,
                FullContent = this.sanitizer.Sanitize(post.FullContent),
                IsDeleted = post.IsDeleted,
                Title = post.Title,
                Tags = string.Join(", ", post.Tags.Select(p => p.Tag.Name))
            };

            return postModel;
        }

        public PostDetailsViewModel GetPostDetailById(int id)
        {
            var post = this.postRepo.All().SingleOrDefault(p => p.Id == id);

            if (post == null || post.IsDeleted == true)
            {
                throw new InvalidOperationException("Post not found.");
            }

            var postDetail = new PostDetailsViewModel
            {
                Author = post.Author.UserName,
                Title = this.sanitizer.Sanitize(post.Title),
                FullContent = this.sanitizer.Sanitize(post.FullContent),
                CreatedOn = post.CreatedOn.ToString("dd-MM-yyyy HH:mm"),
                Tags = string.Join(", ", post.Tags.Select(t => t.Tag.Name)),
                Id = post.Id,
                IsDeleted = post.IsDeleted,
                Comments = post.Comments.Where(c => c.IsDeleted == false).Select(c => new CommentViewModel
                {
                    AuthorName = c.Author.UserName,
                    Content = this.sanitizer.Sanitize(c.Content),
                    CreatedOn = c.CreatedOn.ToString("dd-MM-yyyy HH:mm"),
                    Id = c.Id,
                    IsDeleted = c.IsDeleted
                }).OrderByDescending(c => c.CreatedOn).ToList()
            };

            return postDetail;
        }

        public ICollection<PostIndexViewModel> GetPostsByTag(string tagName)
        {
            var posts = this.postRepo.All().Where(p => p.Tags.Any(t => t.Tag.Name == tagName) && p.IsDeleted == false);

            if (posts.Count() == 0)
            {
                throw new InvalidOperationException("No mathcing posts are found.");
            }

            var modelPosts = this.mapper.ProjectTo<PostIndexViewModel>(posts).OrderByDescending(p => p.CreatedOn).ToList();

            return modelPosts;
        }

        public async Task MarkCommentAsDeleted(EditCommentViewModel model)
        {
            var comment = await this.commentRepo.GetByIdAsync(model.CommentId);

            comment.IsDeleted = true;

            this.commentRepo.Update(comment);

            try
            {
                await this.commentRepo.SaveChangesAsync();
            }
            catch (Exception e)
            {
                this.logger.LogDebug(e.Message);

                throw new InvalidOperationException("Sorry, an error occurred while trying to delete your comment.");
            }
        }

        public async Task MarkPostAsDeleted(PostInputViewModel model)
        {
            var post = await this.postRepo.GetByIdAsync(model.Id);

            post.IsDeleted = true;

            this.postRepo.Update(post);

            try
            {
                await this.postRepo.SaveChangesAsync();
            }
            catch (Exception e)
            {
                this.logger.LogDebug(e.Message);

                throw new InvalidOperationException("Sorry, an error occurred while trying to delete your post");
            }
        }

        public async Task UpdateComment(EditCommentViewModel model)
        {
            var content = model.Content;

            if (string.IsNullOrWhiteSpace(content))
            {
                throw new NullReferenceException("Comment is required.");
            }

            var comment = new Comment
            {
                AuthorId = model.CommentAuthorId,
                Id = model.CommentId,
                Content = model.Content,
                CreatedOn = DateTime.Parse(model.CreatedOn),
                IsDeleted = model.IsDeleted,
                PostId = model.PostId
            };

            this.commentRepo.Update(comment);

            try
            {
                await this.commentRepo.SaveChangesAsync();
            }
            catch (Exception e)
            {
                this.logger.LogDebug(e.Message);

                throw new InvalidOperationException("Sorry an error occurred whille trying to edit your comment.");
            }
        }

        public async Task UpdatePost(PostInputViewModel model)
        {
            var content = model.FullContent.Trim();

            if (string.IsNullOrWhiteSpace(content))
            {
                throw new NullReferenceException("Comment is required.");
            }

            var title = model.Title.Trim();

            if (string.IsNullOrWhiteSpace(title))
            {
                throw new NullReferenceException("Title is required.");
            }

            var inputTags = model.Tags.Trim();

            if (string.IsNullOrWhiteSpace(inputTags))
            {
                throw new NullReferenceException("Tag is required.");
            }

            var dataTags = this.tagPostsRepo.All().AsNoTracking().Select(t => t.Tag.Name).ToList();

            var tags = model.Tags.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var tag in tags)
            {
                if (!dataTags.Any(t => t.Equals(tag, StringComparison.InvariantCultureIgnoreCase)))
                {
                    throw new InvalidOperationException($"Invalid tag {tag}");
                }
            }

            var authorId = this.userRepo.All().SingleOrDefault(u => u.UserName == model.Author).Id;

            var post = new Post
            {
                Id = model.Id,
                AuthorId = authorId,
                CreatedOn = model.CreatedOn,
                FullContent = model.FullContent,
                Title = model.Title,
            };

            this.postRepo.Update(post);

            try
            {
                await this.postRepo.SaveChangesAsync();

                var tagsPerPost = tagPostsRepo.All().Where(tp => tp.PostId == post.Id);

                this.tagPostsRepo.DeleteRange(tagsPerPost);

                foreach (var tag in tags)
                {
                    var tagId = this.mapper.ProjectTo<TagInputViewModel>(tagRepo.All()).SingleOrDefault(t => t.Name == tag).Id;

                    var tagPosts = new TagPosts
                    {
                        TagId = tagId,
                        PostId = post.Id
                    };

                    post.Tags.Add(tagPosts);

                }

                await this.postRepo.SaveChangesAsync();

            }
            catch (Exception e)
            {
                this.logger.LogDebug(e.Message);

                throw new InvalidOperationException("Sorry. An error occurred while updating your post.");
            }

        }

        private string GetShortContent(string fullContent)
        {
            var content = this.sanitizer.Sanitize(this.StripHtml(fullContent));

            var contentLength = content.Length;

            return content.Substring(0, (contentLength < 100 ? contentLength : 100));
        }

        private string StripHtml(string source)
        {
            string output;

            //get rid of HTML tags
            output = Regex.Replace(source, "<[^>]*>", string.Empty);

            //get rid of multiple blank lines
            output = Regex.Replace(output, @"^\s*$\n", string.Empty, RegexOptions.Multiline);

            return HttpUtility.HtmlDecode(output);
        }

        public IEnumerable<PostIndexViewModel> GetAllPosts()
        {
            var posts = this.postRepo.All();

            var modelPosts = posts.Select(p => new PostIndexViewModel
            {
                Author = p.Author.UserName,
                FullContent = GetShortContent(p.FullContent),
                CommentCount = p.Comments.Count,
                CreatedOn = p.CreatedOn.ToString("dd-MM-yyyy HH:mm"),
                Tags = string.Join(", ", p.Tags.Select(t => t.Tag.Name)),
                Title = this.sanitizer.Sanitize(p.Title),
                Id = p.Id,
                IsDeleted = p.IsDeleted
            }).OrderByDescending(p => p.CreatedOn).ToList();

            return modelPosts;
        }
    }
}
