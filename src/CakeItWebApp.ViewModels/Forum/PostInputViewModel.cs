using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CakeItWebApp.ViewModels.Forum
{
    public class PostInputViewModel
    {
        public int Id { get; set; }

        public string Author { get; set; }

        [Required, MinLength(10)]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Full Name")]
        public string FullContent { get; set; }

        public DateTime CreatedOn => DateTime.UtcNow;

        public bool IsDeleted { get; set; } = false;

        [Required]
        public string Tags { get; set; }
    }
}
