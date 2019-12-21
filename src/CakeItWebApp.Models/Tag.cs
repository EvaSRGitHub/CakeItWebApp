using System.Collections.Generic;

namespace CakeItWebApp.Models
{
    public class Tag
    {
        public Tag()
        {
            this.Posts = new HashSet<TagPosts>();
            this.IsDeleted = false;
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public bool  IsDeleted { get; set; }

        public virtual ICollection<TagPosts> Posts { get; set; }
    }
}
