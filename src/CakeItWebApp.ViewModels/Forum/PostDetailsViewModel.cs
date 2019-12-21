using System.Collections.Generic;

namespace CakeItWebApp.ViewModels.Forum
{
    public class PostDetailsViewModel
    {
        public PostDetailsViewModel()
        {
            this.Comments = new HashSet<CommentViewModel>();
        }

        public int Id { get; set; }

        public string Title { get; set; }

        public string FullContent { get; set; }

        public string Tags { get; set; }

        public string Author { get; set; }

        public string CreatedOn { get; set; }

        public bool IsDeleted { get; set; }

        public ICollection<CommentViewModel> Comments { get; set; }

        public CommentInputViewModel CreateComment { get; set; }
       

    }
}
