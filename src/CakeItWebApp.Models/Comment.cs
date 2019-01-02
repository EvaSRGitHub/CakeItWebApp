using System;

namespace CakeItWebApp.Models
{
    public class Comment
    {
        public int Id { get; set; }

        public string AuthorId { get; set; }

        public virtual CakeItUser Author { get; set; }

        public int PostId { get; set; }

        public virtual Post Post { get; set; }

        public string Content { get; set; }

        public DateTime CreatedOn { get; set; }

        public bool IsDeleted { get; set; }
    }
}