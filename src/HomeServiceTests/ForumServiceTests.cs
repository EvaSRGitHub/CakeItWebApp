using CakeItWebApp.Data;
using CakeItWebApp.Models;
using CakeItWebApp.Services.Common.Repository;
using CakeItWebApp.ViewModels.Forum;
using CakeWebApp.Services.Common.CommonServices;
using CakeWebApp.Services.Common.Contracts;
using CakeWebApp.Services.Common.Sanitizer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CakeItWebApp.Services.Common.Tests
{

    public class ForumServiceTests : BaseServiceTestClass
    {
        private ILogger<ForumService> logger;
        private IRepository<Post> postRepo;
        private IRepository<Comment> commentRepo;
        private IRepository<CakeItUser> userRepo;
        private IRepository<Tag> tagRepo;
        private IRepository<TagPosts> tagPostsRepo;
        private ISanitizer sanitizer;

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

        private async Task<IForumService> Setup(CakeItDbContext db)
        {
            var mockLogger = new Mock<ILogger<ForumService>>();
            this.logger = mockLogger.Object;
            this.postRepo = new Repository<Post>(db);
            this.commentRepo = new Repository<Comment>(db);
            this.userRepo = new Repository<CakeItUser>(db);
            await SeedUsers(userRepo);

            this.tagRepo = new Repository<Tag>(db);
            await SeedTags(tagRepo);

            this.tagPostsRepo = new Repository<TagPosts>(db);
            var mockSanitizer = new Mock<HtmlSanitizerAdapter>();
            this.sanitizer = mockSanitizer.Object;

            var forumService = new ForumService(logger, postRepo, commentRepo, userRepo, tagRepo, tagPostsRepo, this.Mapper, sanitizer);

            return forumService;
        }

        [Fact]
        public async Task CreatePost_WithValidData_ShouldAddPostToDb()
        {
            //Arrange
            var db = this.SetDb();

            var service = await this.Setup(db);

            var postModel = new PostInputViewModel
            {
                Title = "Test Post",
                Author = "eva@abv.bg",
                FullContent = "Some test post content",
                Tags = "Baking, Cakes"
            };

            //Act
            await service.CreatePost(postModel);
            var acutalPostCount = this.postRepo.All().Count();
            var expecteddPostCount = 1;

            //Assert
            Assert.Equal(expecteddPostCount, acutalPostCount);
        }

        [Fact]
        public async Task CreatePost_WithEmptyContent_ShouldThrow()
        {
            //Arrange
            var db = this.SetDb();

            var service = await this.Setup(db);

            var postModel = new PostInputViewModel
            {
                Title = "Test Post",
                Author = "eva@abv.bg",
                FullContent = "",
                Tags = "Baking, Cakes"
            };

            //Act

            //Assert
            await Assert.ThrowsAsync<NullReferenceException>(async () => await service.CreatePost(postModel));
        }

        [Fact]
        public async Task CreatePost_WithEmptyTitle_ShouldThrow()
        {
            //Arrange
            var db = this.SetDb();

            var service = await this.Setup(db);

            var postModel = new PostInputViewModel
            {
                Title = "",
                Author = "eva@abv.bg",
                FullContent = "Some test content",
                Tags = "Baking, Cakes"
            };

            //Act

            //Assert
            await Assert.ThrowsAsync<NullReferenceException>(async () => await service.CreatePost(postModel));
        }

        [Fact]
        public async Task CreatePost_WithEmptyTag_ShouldThrow()
        {
            //Arrange
            var db = this.SetDb();

            var service = await this.Setup(db);

            var postModel = new PostInputViewModel
            {
                Title = "Test Post",
                Author = "eva@abv.bg",
                FullContent = "Some test content.",
                Tags = ""
            };

            //Act

            //Assert
            await Assert.ThrowsAsync<NullReferenceException>(async () => await service.CreatePost(postModel));
        }

        [Fact]
        public async Task CreatePost_WithDuplicateTitle_ShouldThrow()
        {
            //Arrange
            var db = this.SetDb();

            var service = await this.Setup(db);

            var postModelA = new PostInputViewModel
            {
                Title = "Test Post",
                Author = "eva@abv.bg",
                FullContent = "Some test post content",
                Tags = "Baking, Cakes"
            };
            await service.CreatePost(postModelA);

            var postModelB = new PostInputViewModel
            {
                Title = "Test Post",
                Author = "eva@abv.bg",
                FullContent = "Some test postB content",
                Tags = "Baking"
            };

            //Act

            //Assert
            await Assert.ThrowsAsync<InvalidOperationException
>(async () => await service.CreatePost(postModelB));
        }

        [Fact]
        public async Task CreateComment_WithValidData_ShouldAddCommentToDb()
        {
            //Arrange
            var db = this.SetDb();

            var service = await this.Setup(db);

            var postModel = new PostInputViewModel
            {
                Title = "Test Post",
                Author = "eva@abv.bg",
                FullContent = "Some test post content",
                Tags = "Baking, Cakes"
            };

            await service.CreatePost(postModel);

            var commentModel = new CommentInputViewModel
            {
                AuthorName = "otherUser@abv.bg",
                Content = "Test comment",
                PostId = 1
            };

            //Act
            await service.CreateComment(commentModel);
            var expectedCommentsCount = 1;
            var actualCommentsCount = this.commentRepo.All().Count();

            //Assert
            Assert.Equal(expectedCommentsCount, actualCommentsCount);
        }

        [Fact]
        public async Task CreateComment_WithValidData_ShouldAddCommentPost()
        {
            //Arrange
            var db = this.SetDb();

            var service = await this.Setup(db);

            var postModel = new PostInputViewModel
            {
                Title = "Test Post",
                Author = "eva@abv.bg",
                FullContent = "Some test post content",
                Tags = "Baking, Cakes"
            };

            await service.CreatePost(postModel);

            var commentModel = new CommentInputViewModel
            {
                AuthorName = "otherUser@abv.bg",
                Content = "Test comment",
                PostId = 1
            };

            //Act
            await service.CreateComment(commentModel);
            var comment = this.commentRepo.All().First();

            //Assert
            Assert.Contains<Comment>(comment, postRepo.All().First().Comments);
        }

        [Fact]
        public async Task CreateComment_WithEmptyContent_ShouldThrow()
        {
            //Arrange
            var db = this.SetDb();

            var service = await this.Setup(db);

            var postModel = new PostInputViewModel
            {
                Title = "Test Post",
                Author = "eva@abv.bg",
                FullContent = "Some test post content",
                Tags = "Baking, Cakes"
            };

            await service.CreatePost(postModel);

            var commentModel = new CommentInputViewModel
            {
                AuthorName = "otherUser@abv.bg",
                Content = "",
                PostId = 1
            };

            //Act

            //Assert
            await Assert.ThrowsAsync<NullReferenceException>(async () => await service.CreateComment(commentModel));
        }

        [Fact]
        public async Task CreateComment_WithWhiteSpaceContent_ShouldThrow()
        {
            //Arrange
            var db = this.SetDb();

            var service = await this.Setup(db);

            var postModel = new PostInputViewModel
            {
                Title = "Test Post",
                Author = "eva@abv.bg",
                FullContent = "Some test post content",
                Tags = "Baking, Cakes"
            };

            await service.CreatePost(postModel);

            var commentModel = new CommentInputViewModel
            {
                AuthorName = "otherUser@abv.bg",
                Content = "     ",
                PostId = 1
            };

            //Act

            //Assert
            await Assert.ThrowsAsync<NullReferenceException>(async () => await service.CreateComment(commentModel));
        }

        [Fact]
        public async Task CreateComment_WithNullComment_ShouldThrow()
        {
            //Arrange
            var db = this.SetDb();

            var service = await this.Setup(db);

            var postModel = new PostInputViewModel
            {
                Title = "Test Post",
                Author = "eva@abv.bg",
                FullContent = "Some test post content",
                Tags = "Baking, Cakes"
            };

            await service.CreatePost(postModel);

            CommentInputViewModel commentModel = null;

            //Act

            //Assert
            await Assert.ThrowsAsync<NullReferenceException>(async () => await service.CreateComment(commentModel));
        }

        [Fact]
        public async Task GetAllCommentsPerPost_WithValidPost_ShouldReturnAllComments()
        {
            //Arrange
            var db = this.SetDb();

            var service = await this.Setup(db);

            var postModel = new PostInputViewModel
            {
                Title = "Test Post",
                Author = "eva@abv.bg",
                FullContent = "Some test post content",
                Tags = "Baking, Cakes"
            };

            await service.CreatePost(postModel);

            var commentModel = new CommentInputViewModel
            {
                AuthorName = "otherUser@abv.bg",
                Content = "Test comment",
                PostId = 1
            };

            await service.CreateComment(commentModel);

            //Act
            var result = await service.GetAllCommentsPerPost(1);

            var expectedCount = 1;
            var actualCount = result.Comments.Count();

            //Assert
            Assert.Equal(expectedCount, actualCount);
        }

        [Fact]
        public async Task GetAllMyPosts_WithPosts_ShouldReturnUserPosts()
        {
            //Arrange 
            var db = this.SetDb();

            var service = await this.Setup(db);

            var postModel = new PostInputViewModel
            {
                Title = "Test Post",
                Author = "eva@abv.bg",
                FullContent = "Some test post content",
                Tags = "Baking, Cakes"
            };

            await service.CreatePost(postModel);

            //Act
            var posts = service.GetAllMyPosts("eva@abv.bg");

            //Assert
            Assert.NotEmpty(posts);
                                    Assert.IsAssignableFrom<IQueryable<UserPostsViewModel>>(posts);
        }

        [Fact]
        public async Task GetAllMyPosts_WithNoPosts_ShouldThrow()
        {
            //Arrange 
            var db = this.SetDb();

            var service = await this.Setup(db);

            //Act

            //Assert
            Assert.Throws<InvalidOperationException>(() => service.GetAllMyPosts("eva@abv.bg"));
        }

        [Fact]
        public async Task GetAllMyComments_WithComments_ShouldReturnUserComment()
        {
            //Arrange
            var db = this.SetDb();

            var service = await this.Setup(db);

            var postModel = new PostInputViewModel
            {
                Title = "Test Post",
                Author = "eva@abv.bg",
                FullContent = "Some test post content",
                Tags = "Baking, Cakes"
            };

            await service.CreatePost(postModel);

            var commentModel = new CommentInputViewModel
            {
                AuthorName = "otherUser@abv.bg",
                Content = "Test comment",
                PostId = 1
            };

            await service.CreateComment(commentModel);

            //Act
            var comments = service.GetAllMyComments("otherUser@abv.bg");

            //Assert
            Assert.NotEmpty(comments);
        }

        [Fact]
        public async Task GetAllMyComments_WithNoComments_ShouldThrow()
        {
            //Arrange
            var db = this.SetDb();

            var service = await this.Setup(db);

            var postModel = new PostInputViewModel
            {
                Title = "Test Post",
                Author = "eva@abv.bg",
                FullContent = "Some test post content",
                Tags = "Baking, Cakes"
            };

            await service.CreatePost(postModel);

            //Act

            //Assert
            Assert.Throws<InvalidOperationException>(() => service.GetAllMyComments("otherUser@abv.bg"));
        }

        [Fact]
        public async Task GetAllActivePosts_WithNoSearchString_ShouldReturnAllPosts()
        {
            //Arrange
            var db = this.SetDb();

            var service = await this.Setup(db);

            var postModelA = new PostInputViewModel
            {
                Title = "Test Post",
                Author = "eva@abv.bg",
                FullContent = "Some test post content",
                Tags = "Baking, Cakes"
            };

            await service.CreatePost(postModelA);

            var postModelB = new PostInputViewModel
            {
                Title = "Test Post 2",
                Author = "otherUser@abv.bg",
                FullContent = "Some test post content",
                Tags = "Baking"
            };

            await service.CreatePost(postModelB);

            //Act
            var posts = service.GetAllActivePosts(null);

            //Assert
            Assert.NotEmpty(posts);
        }

        [Fact]
        public async Task GetAllActivePosts_WithSearchStringSmollLetters_ShouldReturnPosts()
        {
            //Arrange
            var db = this.SetDb();

            var service = await this.Setup(db);

            var postModelA = new PostInputViewModel
            {
                Title = "Test Post",
                Author = "eva@abv.bg",
                FullContent = "Some test post content",
                Tags = "Baking, Cakes"
            };

            await service.CreatePost(postModelA);

            var postModelB = new PostInputViewModel
            {
                Title = "Post 2",
                Author = "otherUser@abv.bg",
                FullContent = "Some test post content",
                Tags = "Baking"
            };

            await service.CreatePost(postModelB);

            //Act
            var posts = service.GetAllActivePosts("test");

            var expectedPostId = 1;
            var actualPostId = posts.First().Id;

            //Assert
            Assert.Equal(expectedPostId, actualPostId);
        }

        [Fact]
        public async Task GetCommentToEditOrDelete_WithValidCommentId_ShouldReturnComment()
        {
            //Arrange
            var db = this.SetDb();

            var service = await this.Setup(db);

            var postModel = new PostInputViewModel
            {
                Title = "Test Post",
                Author = "eva@abv.bg",
                FullContent = "Some test post content",
                Tags = "Baking, Cakes"
            };

            await service.CreatePost(postModel);

            var commentModel = new CommentInputViewModel
            {
                AuthorName = "otherUser@abv.bg",
                Content = "Test comment",
                PostId = 1
            };

            await service.CreateComment(commentModel);

            //Act
            var comment = service.GetCommentToEditOrDelete(1);

            var expectedCommentId = 1;
            var actualCommentId = comment.Id;

            //Assert
            Assert.Equal(expectedCommentId, actualCommentId);
        }

        [Fact]
        public async Task GetCommentToEditOrDelete_WithInValidCommentId_ShouldThrow()
        {
            //Arrange
            var db = this.SetDb();

            var service = await this.Setup(db);

            var postModel = new PostInputViewModel
            {
                Title = "Test Post",
                Author = "eva@abv.bg",
                FullContent = "Some test post content",
                Tags = "Baking, Cakes"
            };

            await service.CreatePost(postModel);

            var commentModel = new CommentInputViewModel
            {
                AuthorName = "otherUser@abv.bg",
                Content = "Test comment",
                PostId = 1
            };

            await service.CreateComment(commentModel);

            //Act

            //Assert
            await Assert.ThrowsAsync<NullReferenceException>(async () => await service.GetCommentToEditOrDelete(2));
        }

        [Fact]
        public async Task GetCommentToEditOrDelete_WithDeletedPost_ShouldThrow()
        {
            //Arrange
            var db = this.SetDb();

            var service = await this.Setup(db);

            var postModel = new PostInputViewModel
            {
                Title = "Test Post",
                Author = "eva@abv.bg",
                FullContent = "Some test post content",
                Tags = "Baking, Cakes"
            };

            await service.CreatePost(postModel);

            var commentModel = new CommentInputViewModel
            {
                AuthorName = "otherUser@abv.bg",
                Content = "Test comment",
                PostId = 1
            };

            await service.CreateComment(commentModel);

            var post = await this.postRepo.GetByIdAsync(1);
            post.IsDeleted = true;
            await this.postRepo.SaveChangesAsync();

            //Act

            //Assert
            await Assert.ThrowsAsync<InvalidOperationException
>(async () => await service.GetCommentToEditOrDelete(1));
        }

        [Fact]
        public async Task GetPostById_WithValidId_ShouldReturnPost()
        {
            //Arrange
            var db = this.SetDb();

            var service = await this.Setup(db);

            var postModelA = new PostInputViewModel
            {
                Title = "Test Post A",
                Author = "eva@abv.bg",
                FullContent = "Some test post content",
                Tags = "Baking, Cakes"
            };

            await service.CreatePost(postModelA);

            var postModelB = new PostInputViewModel
            {
                Title = "Test Post B",
                Author = "otherUser@abv.bg",
                FullContent = "Some test post content",
                Tags = "Cakes"
            };

            await service.CreatePost(postModelB);

            //Act
            var post = await service.GetPostById(2);

            var expectedPostId = 2;
            var actualPostId = post.Id;

            //Assert
            Assert.Equal(expectedPostId, actualPostId);
        }

        [Fact]
        public async Task GetPostById_WithInValidId_ShouldThrow()
        {
            //Arrange
            var db = this.SetDb();

            var service = await this.Setup(db);

            var postModelA = new PostInputViewModel
            {
                Title = "Test Post A",
                Author = "eva@abv.bg",
                FullContent = "Some test post content",
                Tags = "Baking, Cakes"
            };

            await service.CreatePost(postModelA);

            //Act

            //Assert
            await Assert.ThrowsAsync<NullReferenceException>(async () => await service.GetPostById(2));
        }

        [Fact]
        public async Task GetPostById_WithDeletedPost_ShouldThrow()
        {
            //Arrange
            var db = this.SetDb();

            var service = await this.Setup(db);

            var postModelA = new PostInputViewModel
            {
                Title = "Test Post A",
                Author = "eva@abv.bg",
                FullContent = "Some test post content",
                Tags = "Baking, Cakes"
            };

            await service.CreatePost(postModelA);

            var post = await this.postRepo.GetByIdAsync(1);

            post.IsDeleted = true;

            await this.postRepo.SaveChangesAsync();

            //Act

            //Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await service.GetPostById(1
));
        }

        [Fact]
        public async Task GetPostDetailById_WithCommentValidId_ShouldReturnPostDetails()
        {
            //Arrange
            var db = this.SetDb();

            var service = await this.Setup(db);

            var postModelA = new PostInputViewModel
            {
                Title = "Test Post A",
                Author = "eva@abv.bg",
                FullContent = "Some test post content",
                Tags = "Baking, Cakes"
            };

            await service.CreatePost(postModelA);

            var commentModel = new CommentInputViewModel
            {
                AuthorName = "otherUser@abv.bg",
                Content = "Test comment",
                PostId = 1
            };

            await service.CreateComment(commentModel);

            //Act
            var post = service.GetPostDetailById(1);

            var expectedComments = 1;
            var actualComments = post.Comments.Count();

            //Assert
            Assert.NotNull(post);
            Assert.Equal(expectedComments, actualComments);
            Assert.IsType<PostDetailsViewModel>(post);
        }

        [Fact]
        public async Task GetPostDetailById_WithInValidId_ShouldThrow()
        {
            //Arrange
            var db = this.SetDb();

            var service = await this.Setup(db);

            var postModelA = new PostInputViewModel
            {
                Title = "Test Post A",
                Author = "eva@abv.bg",
                FullContent = "Some test post content",
                Tags = "Baking, Cakes"
            };

            await service.CreatePost(postModelA);

            //Act

            //Assert
            Assert.Throws<InvalidOperationException>(() => service.GetPostDetailById(2));
        }

        [Fact]
        public async Task GetPostDetailById_WithDeletedPost_ShouldThrow()
        {
            //Arrange
            var db = this.SetDb();

            var service = await this.Setup(db);

            var postModelA = new PostInputViewModel
            {
                Title = "Test Post A",
                Author = "eva@abv.bg",
                FullContent = "Some test post content",
                Tags = "Baking, Cakes"
            };

            await service.CreatePost(postModelA);

            var post = await this.postRepo.GetByIdAsync(1);

            post.IsDeleted = true;

            await this.postRepo.SaveChangesAsync();

            //Act

            //Assert
            Assert.Throws<InvalidOperationException>(() => service.GetPostDetailById(1));
        }

        [Fact]
        public async Task GetPostsByTag_WithValidTag_ShouldReturnTwoPostsWithTag()
        {
            //Arrange
            var db = this.SetDb();

            var service = await this.Setup(db);

            var postModelA = new PostInputViewModel
            {
                Title = "Test Post A",
                Author = "eva@abv.bg",
                FullContent = "Some test post content",
                Tags = "Baking, Cakes"
            };

            await service.CreatePost(postModelA);

            var postModelB = new PostInputViewModel
            {
                Title = "Test Post B",
                Author = "otherUser@abv.bg",
                FullContent = "Some test post content",
                Tags = "Baking"
            };

            await service.CreatePost(postModelB);

            //Act
            var posts = service.GetPostsByTag("Baking");

            var expectedPostCount = 2;
            var actualPostCount = posts.Count();

            //Assert
            Assert.Equal(expectedPostCount, actualPostCount);
        }

        [Fact]
        public async Task GetPostsByTag_WithValidTag_ShouldReturnOnePostsWithTag()
        {
            //Arrange
            var db = this.SetDb();

            var service = await this.Setup(db);

            var postModelA = new PostInputViewModel
            {
                Title = "Test Post A",
                Author = "eva@abv.bg",
                FullContent = "Some test post content",
                Tags = "Cakes"
            };

            await service.CreatePost(postModelA);

            var postModelB = new PostInputViewModel
            {
                Title = "Test Post B",
                Author = "otherUser@abv.bg",
                FullContent = "Some test post content",
                Tags = "Baking"
            };

            await service.CreatePost(postModelB);

            //Act
            var posts = service.GetPostsByTag("Baking");

            var expectedPostCount = 1;
            var actualPostCount = posts.Count();

            //Assert
            Assert.Equal(expectedPostCount, actualPostCount);
        }

        [Fact]
        public async Task GetPostsByTag_WithInValidTag_ShouldThrow()
        {
            //Arrange
            var db = this.SetDb();

            var service = await this.Setup(db);

            var postModelA = new PostInputViewModel
            {
                Title = "Test Post A",
                Author = "eva@abv.bg",
                FullContent = "Some test post content",
                Tags = "Cakes"
            };

            await service.CreatePost(postModelA);

            //Act

            //Assert
            Assert.Throws<InvalidOperationException>(() => service.GetPostsByTag("Sponge"));
        }

        [Fact]
        public async Task GetPostsByTag_WithDeletedPost_ShouldThrow()
        {
            //Arrange
            var db = this.SetDb();

            var service = await this.Setup(db);

            var postModelA = new PostInputViewModel
            {
                Title = "Test Post A",
                Author = "eva@abv.bg",
                FullContent = "Some test post content",
                Tags = "Cakes"
            };

            await service.CreatePost(postModelA);

            var post = await this.postRepo.GetByIdAsync(1);

            post.IsDeleted = true;

            await this.postRepo.SaveChangesAsync();
            //Act

            //Assert
            Assert.Throws<InvalidOperationException>(() => service.GetPostsByTag("Cakes"));
        }

        [Fact]
        public async Task MarkCommentAsDeleted_WithValidModel_ShouldMarkCommentAsDeleted()
        {
            //Arrange
            var db = this.SetDb();

            var service = await this.Setup(db);

            var postModel = new PostInputViewModel
            {
                Title = "Test Post",
                Author = "eva@abv.bg",
                FullContent = "Some test post content",
                Tags = "Baking, Cakes"
            };

            await service.CreatePost(postModel);

            var commentModel = new CommentInputViewModel
            {
                AuthorName = "otherUser@abv.bg",
                Content = "Test comment",
                PostId = 1
            };

            await service.CreateComment(commentModel);

            var comment = await this.commentRepo.GetByIdAsync(1);

            var model = this.Mapper.Map<Comment, EditCommentViewModel>(comment);
            model.FullContent = this.sanitizer.Sanitize(model.FullContent);
            model.Content = this.sanitizer.Sanitize(model.Content);

            //Act
            await service.MarkCommentAsDeleted(model);

            var expectedCommentIsDeleted = true;
            var actualCommentIsDeleted = comment.IsDeleted;

            //Assert
            Assert.Equal(expectedCommentIsDeleted, actualCommentIsDeleted);
        }

        [Fact]
        public async Task UpdateComment_WithValidModel_ShouldSaveUpdatedComment()
        {
            //Arrange
            var db = this.SetDb();

            var service = await this.Setup(db);

            var postModel = new PostInputViewModel
            {
                Title = "Test Post",
                Author = "eva@abv.bg",
                FullContent = "Some test post content",
                Tags = "Baking, Cakes"
            };

            await service.CreatePost(postModel);

            var commentModel = new CommentInputViewModel
            {
                AuthorName = "otherUser@abv.bg",
                Content = "Test comment content",
                PostId = 1
            };

            await service.CreateComment(commentModel);

            var comment = await this.commentRepo.GetByIdAsync(1);

            db.Entry(comment).State = EntityState.Detached;

            var model = this.Mapper.Map<Comment, EditCommentViewModel>(comment);
            model.FullContent = model.FullContent;
            model.Content = "Edit content.";

            //Act
            await service.UpdateComment(model);

            var expectedCommentContent = "Edit content.";
            var actualCommentContent = (await this.commentRepo.GetByIdAsync(1)).Content;

            //Assert
            Assert.Equal(expectedCommentContent, actualCommentContent);
        }

        [Fact]
        public async Task UpdateComment_WithEmptyContent_ShouldThrow()
        {
            //Arrange
            var db = this.SetDb();

            var service = await this.Setup(db);

            var postModel = new PostInputViewModel
            {
                Title = "Test Post",
                Author = "eva@abv.bg",
                FullContent = "Some test post content",
                Tags = "Baking, Cakes"
            };

            await service.CreatePost(postModel);

            var commentModel = new CommentInputViewModel
            {
                AuthorName = "otherUser@abv.bg",
                Content = "Test comment content",
                PostId = 1
            };

            await service.CreateComment(commentModel);

            var comment = await this.commentRepo.GetByIdAsync(1);

            db.Entry(comment).State = EntityState.Detached;

            var model = this.Mapper.Map<Comment, EditCommentViewModel>(comment);
            model.Content = "";

            //Act

            //Assert
            await Assert.ThrowsAsync<NullReferenceException>(async () => await service.UpdateComment(model));
        }

        [Fact]
        public async Task UpdatePost_WithValidTitle_ShouldSaveUpdatedPost()
        {
            //Arrange
            var db = this.SetDb();

            var service = await this.Setup(db);

            var postModel = new PostInputViewModel
            {
                Title = "Test Post",
                Author = "eva@abv.bg",
                FullContent = "Some test post content",
                Tags = "Baking, Cakes"
            };

            await service.CreatePost(postModel);

            var post = await this.postRepo.GetByIdAsync(1);

            db.Entry(post).State = EntityState.Detached;

            var model = new PostInputViewModel
            {
                Id = post.Id,
                Author = post.Author.UserName,
                Title = "Edit Title",
                FullContent = post.FullContent,
                Tags = string.Join(", ", post.Tags.Select(tp => tp.Tag.Name)),
                IsDeleted = post.IsDeleted
            };

            //Act
            await service.UpdatePost(model);

            var expectedTitle = "Edit Title";
            var acutalTitle = (await this.postRepo.GetByIdAsync(1)).Title;

            //Assert
            Assert.Equal(expectedTitle, acutalTitle);
        }

        [Fact]
        public async Task UpdatePost_WithValidContent_ShouldSaveUpdatedPost()
        {
            //Arrange
            var db = this.SetDb();

            var service = await this.Setup(db);

            var postModel = new PostInputViewModel
            {
                Title = "Test Post",
                Author = "eva@abv.bg",
                FullContent = "Some test post content",
                Tags = "Baking, Cakes"
            };

            await service.CreatePost(postModel);

            var post = await this.postRepo.GetByIdAsync(1);

            db.Entry(post).State = EntityState.Detached;

            var model = new PostInputViewModel
            {
                Id = post.Id,
                Author = post.Author.UserName,
                Title = post.Title,
                FullContent = "Edit Content",
                Tags = string.Join(", ", post.Tags.Select(tp => tp.Tag.Name)),
                IsDeleted = post.IsDeleted
            };

            //Act
            await service.UpdatePost(model);

            var expectedContent = "Edit Content";
            var acutalContent = (await this.postRepo.GetByIdAsync(1)).FullContent;

            //Assert
            Assert.Equal(expectedContent, acutalContent);
        }

        [Fact]
        public async Task UpdatePost_WithValidTags_ShouldSaveUpdatedPost()
        {
            //Arrange
            var db = this.SetDb();

            var service = await this.Setup(db);

            var postModel = new PostInputViewModel
            {
                Title = "Test Post",
                Author = "eva@abv.bg",
                FullContent = "Some test post content",
                Tags = "Baking"
            };

            await service.CreatePost(postModel);

            var post = await this.postRepo.GetByIdAsync(1);

            db.Entry(post).State = EntityState.Detached;

            var model = new PostInputViewModel
            {
                Id = post.Id,
                Author = post.Author.UserName,
                Title = post.Title,
                FullContent = post.FullContent,
                Tags = "Baking, Cakes",
                IsDeleted = post.IsDeleted
            };

            //Act
            await service.UpdatePost(model);

            var expectedTags = "Baking, Cakes";
            var tags = (await this.postRepo.GetByIdAsync(1)).Tags;
            var acutalTags = string.Join(", ", tags.Select(tp => tp.Tag.Name));

            //Assert
            Assert.Equal(expectedTags, acutalTags);
        }

        [Fact]
        public async Task UpdatePost_WithEmptyTitle_ShouldThrow()
        {
            //Arrange
            var db = this.SetDb();

            var service = await this.Setup(db);

            var postModel = new PostInputViewModel
            {
                Title = "Test Post",
                Author = "eva@abv.bg",
                FullContent = "Some test post content",
                Tags = "Baking, Cakes"
            };

            await service.CreatePost(postModel);

            var post = await this.postRepo.GetByIdAsync(1);

            db.Entry(post).State = EntityState.Detached;

            var model = new PostInputViewModel
            {
                Id = post.Id,
                Author = post.Author.UserName,
                Title = "",
                FullContent = post.FullContent,
                Tags = string.Join(", ", post.Tags.Select(tp => tp.Tag.Name)),
                IsDeleted = post.IsDeleted
            };

            //Act

            //Assert
            await Assert.ThrowsAsync<NullReferenceException>(async () => await service.UpdatePost(model));
        }

        [Fact]
        public async Task UpdatePost_WithEmptyTag_ShouldThrow()
        {
            //Arrange
            var db = this.SetDb();

            var service = await this.Setup(db);

            var postModel = new PostInputViewModel
            {
                Title = "Test Post",
                Author = "eva@abv.bg",
                FullContent = "Some test post content",
                Tags = "Baking, Cakes"
            };

            await service.CreatePost(postModel);

            var post = await this.postRepo.GetByIdAsync(1);

            db.Entry(post).State = EntityState.Detached;

            var model = new PostInputViewModel
            {
                Id = post.Id,
                Author = post.Author.UserName,
                Title = post.Title,
                FullContent = post.FullContent,
                Tags = "",
                IsDeleted = post.IsDeleted
            };

            //Act

            //Assert
            await Assert.ThrowsAsync<NullReferenceException>(async () => await service.UpdatePost(model));
        }

        [Fact]
        public async Task UpdatePost_WithEmptyContent_ShouldThrow()
        {
            //Arrange
            var db = this.SetDb();

            var service = await this.Setup(db);

            var postModel = new PostInputViewModel
            {
                Title = "Test Post",
                Author = "eva@abv.bg",
                FullContent = "Some test post content",
                Tags = "Baking, Cakes"
            };

            await service.CreatePost(postModel);

            var post = await this.postRepo.GetByIdAsync(1);

            db.Entry(post).State = EntityState.Detached;

            var model = new PostInputViewModel
            {
                Id = post.Id,
                Author = post.Author.UserName,
                Title = post.Title,
                FullContent = "",
                Tags = string.Join(", ", post.Tags.Select(tp => tp.Tag.Name)),
                IsDeleted = post.IsDeleted
            };

            //Act

            //Assert
            await Assert.ThrowsAsync<NullReferenceException>(async () => await service.UpdatePost(model));
        }

        [Fact]
        public async Task GetAllPosts_ShouldGetDeletedAndNotDeletedPosts()
        {
            //Arrange
            var db = this.SetDb();

            var service = await this.Setup(db);

            var postModelA = new PostInputViewModel
            {
                Title = "Test Post A",
                Author = "eva@abv.bg",
                FullContent = "Some test post content",
                Tags = "Baking"
            };

            await service.CreatePost(postModelA);

            var postModelB = new PostInputViewModel
            {
                Title = "Test Post B",
                Author = "eva@abv.bg",
                FullContent = "Some test post content",
                Tags = "Baking"
            };

            await service.CreatePost(postModelB);

            var post = await this.postRepo.GetByIdAsync(2);

            post.IsDeleted = true;

            await this.postRepo.SaveChangesAsync();

            //Act
            var allPosts = service.GetAllPosts();

            var expectedPostCount = 2;
            var actualPostCount = allPosts.Count();

            //Assert
            Assert.Equal(expectedPostCount, actualPostCount);
        }

        [Fact]
        public async Task GetAllPosts_WhithNoPosts_ShouldReturnEmpytPostCollection()
        {
            //Arrange
            var db = this.SetDb();

            var service = await this.Setup(db);

            //Act
            var allPosts = service.GetAllPosts();

            //Assert
            Assert.Empty(allPosts);
        }

        private async Task SeedTags(IRepository<Tag> tagRepo)
        {
            tagRepo.Add(new Tag { Id = 1, Name = "Baking" });
            tagRepo.Add(new Tag { Id = 2, Name = "Cakes" });
            await tagRepo.SaveChangesAsync();
        }

        private async Task SeedUsers(IRepository<CakeItUser> userRepo)
        {
            userRepo.Add(new CakeItUser
            {
                Id = "bc117f38-41b6-4e17-b4f9-9789d0609b62",
                UserName = "eva@abv.bg",
                EmailConfirmed = false,
                SecurityStamp = "YUCMGFZ65YAXSTJVN4KCEZGSFJWS7QU2",
                ConcurrencyStamp = "ebc52278-0278-49d7-8036-be79bb9daffe",
                PhoneNumberConfirmed = false,
                TwoFactorEnabled = false,
                LockoutEnabled = false,
                AccessFailedCount = 0,
                CreatedOn = new DateTime(2018, 12, 09, 21, 11, 37),
                IsDeleted = false
            });

            userRepo.Add(new CakeItUser
            {
                Id = "bc118f38-41b6-4e17-b4f9-9789d0609b62",
                UserName = "otherUser@abv.bg",
                EmailConfirmed = false,
                SecurityStamp = "WERMGFZ65YAXSTJVN4KCEZGSFJWS7QU2",
                ConcurrencyStamp = "CBE52278-0278-49d7-8036-be79bb9daffe",
                PhoneNumberConfirmed = false,
                TwoFactorEnabled = false,
                LockoutEnabled = false,
                AccessFailedCount = 0,
                CreatedOn = new DateTime(2018, 12, 10, 21, 11, 37),
                IsDeleted = false
            });
            await userRepo.SaveChangesAsync();
        }
    }
}
