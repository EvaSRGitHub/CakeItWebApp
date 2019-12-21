using System.ComponentModel.DataAnnotations;

namespace CakeItWebApp.ViewModels.Forum
{
    public class UserPostsViewModel
    {
        [Required]
        public int PostId { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public string CreatedOn { get; set; }

        [Required]
        public int CommentsCount { get; set; }
    }
}
