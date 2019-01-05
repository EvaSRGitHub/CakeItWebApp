using System;
using System.Collections.Generic;

namespace CakeItWebApp.Models
{
    public class Post
    {
        public Post()
        {
            this.Tags = new HashSet<TagPosts>();
            this.Comments = new HashSet<Comment>();
        }

        public int Id { get; set; }

        public string AuthorId { get; set; }

        public virtual CakeItUser Author { get; set; }

        public string Title { get; set; }

        public string FullContent { get; set; }

        public DateTime CreatedOn { get; set; }

        public bool IsDeleted { get; set; }

        public virtual ICollection<TagPosts> Tags { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }
    }
}