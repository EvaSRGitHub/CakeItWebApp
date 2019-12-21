using CakeItWebApp.ViewModels.Forum;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CakeWebApp.Services.Common.Contracts
{
    public interface IForumService
    {
        IEnumerable<PostIndexViewModel> GetAllActivePosts(string searchString);

        IEnumerable<PostIndexViewModel> GetAllPosts();

        Task CreatePost(PostInputViewModel model);

        PostDetailsViewModel GetPostDetailById(int id);

        ICollection<PostIndexViewModel> GetPostsByTag(string tagName);

        Task CreateComment(CommentInputViewModel model);

        Task<PostInputViewModel> GetPostById(int id);

        Task UpdatePost(PostInputViewModel model);

        Task MarkPostAsDeleted(PostInputViewModel model);

        Task<EditCommentViewModel> GetCommentToEditOrDelete(int id);

        Task UpdateComment(EditCommentViewModel model);

        Task MarkCommentAsDeleted(EditCommentViewModel model);

        IQueryable<UserPostsViewModel> GetAllMyPosts(string userName);

        IQueryable<CommentInputViewModel> GetAllMyComments(string userName);

        Task<PostDetailsViewModel> GetAllCommentsPerPost(int postId);
    }
}
