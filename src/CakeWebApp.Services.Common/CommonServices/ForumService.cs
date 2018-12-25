using AutoMapper;
using CakeItWebApp.Models;
using CakeItWebApp.Services.Common.Repository;
using CakeItWebApp.ViewModels.Forum;
using CakeWebApp.Services.Common.Contracts;
using CakeWebApp.Services.Common.Sanitizer;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                throw new InvalidOperationException("Sorry, couldn't process your comment.");
            }

            model.AuthorId = this.userRepo.All().SingleOrDefault(u => u.UserName == model.AuthorName).Id;

            var comment = this.mapper.Map<CommentInputViewModel, Comment>(model);

            this.commentRepo.Add(comment);

            try
            {
                await this.commentRepo.SaveChangesAsync();
            }
            catch (Exception e)
            {
                this.logger.LogError(e.Message);

                throw new InvalidOperationException("Sorry, couldn't process your comment.");
            }
        }

        public async Task CreatePost(PostInputViewModel model)
        {
            var authorId = this.userRepo.All().SingleOrDefault(u => u.UserName == model.Author).Id;

            var post = new Post
            {
                AuthorId = authorId,
                CreatedOn = model.CreatedOn,
                FullContent = model.FullContent,
                Title = model.Title,
            };

            this.postRepo.Add(post);

            try
            {
                await this.postRepo.SaveChangesAsync();

                var tags = model.Tags.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

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
                this.logger.LogError(e.Message);

                throw new InvalidOperationException("Sorry. An error occurred while creating a post.");
            }
        }

        public Task EditPost(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<PostIndexViewModel> GetAllPosts()
        {
            var posts = this.postRepo.All().Where(p => p.IsDeleted == false);

            var modelPosts = posts.Select(p => new PostIndexViewModel
            {
                Author = p.Author.UserName,
                CommentCount = p.Comments.Count,
                CreatedOn = p.CreatedOn.ToString("dd-MM-yyyy hh:mm"),
                Tags = string.Join(", ", p.Tags.Select(t => t.Tag.Name)),
                Title = p.Title,
                Id = p.Id
            }).ToList();

            return modelPosts;
        }

        public async Task<PostInputViewModel> GetPostById(int id)
        {
            var post = await this.postRepo.GetByIdAsync(id);

            if (post == null)
            {
                throw new InvalidOperationException("Post not found");
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

            if (post == null && post.IsDeleted == true)
            {
                throw new InvalidOperationException("Post not found.");
            }

            var postDetail = new PostDetailsViewModel
            {
                Author = post.Author.UserName,
                Title = post.Title,
                FullContent = this.sanitizer.Sanitize(post.FullContent),
                CreatedOn = post.CreatedOn.ToString("dd-MM-yyyy hh:mm"),
                Tags = string.Join(", ", post.Tags.Select(t => t.Tag.Name)),
                Id = post.Id,
                IsDeleted = post.IsDeleted,
                Comments = post.Comments.Where(c => c.IsDeleted == false).Select(c => new CommentViewModel
                {
                    // AuthorId = c.AuthorId,
                    Author = c.Author,
                    Content = this.sanitizer.Sanitize(c.Content),
                    CreatedOn = c.CreatedOn.ToString("dd-MM-yyyy hh:mm"),
                    Id = c.Id,
                    IsDeleted = c.IsDeleted
                }).ToList()
            };

            return postDetail;
        }

        public IEnumerable<PostIndexViewModel> GetPostsByTag(string tagName)
        {
            throw new NotImplementedException();
        }

        public async Task MarkPostAsDeleted(PostInputViewModel model)
        {
            var post = await this.postRepo.GetByIdAsync(model.Id);

            if(post == null)
            {
                throw new InvalidOperationException("Post not found.");
            }

            post.IsDeleted = true;

            this.postRepo.Update(post);

            try
            {
                await this.postRepo.SaveChangesAsync();
            }
            catch (Exception e)
            {
                this.logger.LogError(e.Message);

                throw new InvalidOperationException("Sorry, an error occurred while trying to delete your post");
            }
        }

        public async Task UpdatePost(PostInputViewModel model)
        {
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

                var tags = model.Tags.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

                var tagsPerPost = tagPostsRepo.All().Where(tp => tp.PostId == post.Id);

                this.tagPostsRepo.DeleteRange(tagsPerPost);

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
                this.logger.LogError(e.Message);

                throw new InvalidOperationException("Sorry. An error occurred while updating your post.");
            }

        }
    }
}
