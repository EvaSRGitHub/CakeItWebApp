namespace CakeItWebApp.Models
{
    public class TagPosts
    {
        public int TagId { get; set; }

        public virtual Tag Tag { get; set; }

        public int PostId { get; set; }

        public virtual Post Post { get; set; }
    }
}
