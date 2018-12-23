using CakeItWebApp.Models;
using CakeItWebApp.Services.Common.Repository;
using CakeItWebApp.ViewModels.CustomCake.Forum;
using CakeWebApp.Services.Common.Contracts;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace CakeWebApp.Services.Common.CommonServices
{
    public class ForumService : IForumService
    {
        private readonly ILogger<ForumService> logger;
        private readonly IRepository<Post> postRepo;
        private readonly IRepository<Comment> commentRepo;

        public IEnumerable<PostIndexViewModel> GetAllPosts()
        {
            throw new NotImplementedException();
        }
    }
}
