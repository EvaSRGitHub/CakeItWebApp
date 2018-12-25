using CakeItWebApp.ViewModels.Forum;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CakeWebApp.Services.Common.Contracts
{
    public interface IForumService
    {
        IEnumerable<PostIndexViewModel> GetAllPosts();

        Task CreatePost(PostInputViewModel model);

        PostDetailsViewModel GetPostDetailById(int id);

        IEnumerable<PostIndexViewModel> GetPostsByTag(string tagName);

        Task CreateComment(CommentInputViewModel model);

        Task<PostInputViewModel> GetPostById(int id);

        Task EditPost(int id);

        Task UpdatePost(PostInputViewModel model);

        Task MarkPostAsDeleted(PostInputViewModel model);
    }
}
