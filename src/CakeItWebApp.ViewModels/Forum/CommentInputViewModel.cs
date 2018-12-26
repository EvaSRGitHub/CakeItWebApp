using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CakeItWebApp.ViewModels.Forum
{
    public class CommentInputViewModel
    {
        public int Id { get; set; }

        public string AuthorId { get; set; }

        [Required]
        public string AuthorName { get; set; }

        [Required]
        public int PostId { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        [Required]
        public bool IsDeleted { get; set; } = false;
    }
}
