using CakeItWebApp.Models;
using CakeItWebApp.Services.Common.Repository;
using CakeItWebApp.ViewModels.Forum;
using CakeWebApp.Services.Common.CommonServices;
using CakeWebApp.Services.Common.Contracts;
using CakeWebApp.Services.Common.Sanitizer;
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

        private async Task<IForumService> Setup()
        {
            var mockLogger = new Mock<ILogger<ForumService>>();
            this.logger = mockLogger.Object;
            this.postRepo = new Repository<Post>(this.Db);
            this.commentRepo = new Repository<Comment>(this.Db);
            this.userRepo = new Repository<CakeItUser>(this.Db);
            await SeedUsers(userRepo);

            this.tagRepo = new Repository<Tag>(this.Db);
            await SeedTags(tagRepo);

            this.tagPostsRepo = new Repository<TagPosts>(this.Db);
            var mockSanitizer = new Mock<HtmlSanitizerAdapter>();
            this.sanitizer = mockSanitizer.Object;

            var forumService = new ForumService(logger, postRepo, commentRepo, userRepo, tagRepo, tagPostsRepo, this.Mapper, sanitizer);

            return forumService;
        }

        [Fact]
        public async Task CreatePost_WithValidData_ShouldAddPostToDb()
        {
            //Arrange
            var service = await this.Setup();

            var postModel = new PostInputViewModel
            {
                Title = "Test Post",
                Author = "eva@abv.bg",
                FullContent = "Some test post content",
                Tags = "Baking, Cakes"
            };

            //Act
            await service.CreatePost(postModel);
            var acutalPostCount = this.Db.Posts.Count();
            var expecteddPostCount = 1;

            //Assert
            Assert.Equal(expecteddPostCount, acutalPostCount);
        }

        [Fact]
        public async Task CreatePost_WithDuplicateTitle_ShouldThrow()
        {
            //Arrange
            var service = await this.Setup();

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
            var service = await this.Setup();

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
            var service = await this.Setup();

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
            var service = await this.Setup();

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
