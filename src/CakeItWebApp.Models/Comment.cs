namespace CakeItWebApp.Models
{
    public class Comment
    {
        public int Id { get; set; }

        public string AuthorId { get; set; }

        public virtual CakeItUser Author { get; set; }

        public int PostId { get; set; }

        public virtual Post Podt { get; set; }

        public string Content { get; set; }

        public bool IsDeleted { get; set; }
    }
}