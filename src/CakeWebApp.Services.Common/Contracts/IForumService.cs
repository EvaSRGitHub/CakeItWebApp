using CakeItWebApp.ViewModels.CustomCake.Forum;
using System;
using System.Collections.Generic;
using System.Text;

namespace CakeWebApp.Services.Common.Contracts
{
    public interface IForumService
    {
        IEnumerable<PostIndexViewModel> GetAllPosts();
    }
}
