using System.ComponentModel.DataAnnotations;

namespace CakeItWebApp.ViewModels.Forum
{
    public class EditCommentViewModel
    {
        public string PostAuthorName { get; set; }

        public int PostId { get; set; }

        public string FullContent { get; set; }

        public string Title { get; set; }

        public int CommentId { get; set; }

        [Required]
        public string CommentAuthor { get; set; }

        [Required]
        public string CommentAuthorId { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public string CreatedOn { get; set; }

        [Required]
        public bool IsDeleted { get; set; }

    }
}
