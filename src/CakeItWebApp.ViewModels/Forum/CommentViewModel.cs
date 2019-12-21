using System.ComponentModel.DataAnnotations;

namespace CakeItWebApp.ViewModels.Forum
{
    public class CommentViewModel
    {
        [Required]
        public int Id { get; set; }

        public string AuthorId { get; set; }

        [Required]
        public string AuthorName { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public string CreatedOn { get; set; }

        [Required]
        public bool IsDeleted { get; set; }
    }
}
