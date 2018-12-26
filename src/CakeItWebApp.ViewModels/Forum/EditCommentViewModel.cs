using System;
using System.Collections.Generic;
using System.Text;

namespace CakeItWebApp.ViewModels.Forum
{
    public class EditCommentViewModel
    {
        public PostInputViewModel Post { get; set; }

        public CommentInputViewModel Comment { get; set; }
    }
}
