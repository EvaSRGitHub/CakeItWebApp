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
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public async Task GetPostDetailById_WithValidId_ShouldReturnPostDetails()
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
            var post = service.GetPostDetailById(1);

            //Assert
            Assert.NotNull(post);
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
