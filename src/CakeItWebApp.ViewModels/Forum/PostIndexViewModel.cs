using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CakeItWebApp.ViewModels.Forum
{
    public class PostIndexViewModel
    { 
        public int Id { get; set; }

        public string Author { get; set; }

        [Required, MinLength(10)]
        public string Title { get; set; }

        [Required]
        public string FullContent { get; set; }

        [Required]
        public int CommentCount { get; set; } 

        [Required]
        public string CreatedOn { get; set; }

        public bool IsDeleted { get; set; } = false;

        [Required]
        public string Tags { get; set; }
    }
}
