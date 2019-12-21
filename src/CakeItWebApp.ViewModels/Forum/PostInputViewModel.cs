using System;
using System.ComponentModel.DataAnnotations;

namespace CakeItWebApp.ViewModels.Forum
{
    public class PostInputViewModel
    {
        public int Id { get; set; }

        public string Author { get; set; }

        [Required, MinLength(10)]
        public string Title { get; set; }

        [Required, MinLength(2)]
        [Display(Name = "Post Content")]
        public string FullContent { get; set; }

        public DateTime CreatedOn => DateTime.UtcNow;

        public bool IsDeleted { get; set; } = false;

        [Required, MinLength(2)]
        public string Tags { get; set; }
    }
}
