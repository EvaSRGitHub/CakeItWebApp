using System;
using System.Collections.Generic;
using System.Text;

namespace CakeItWebApp.ViewModels.Forum
{
    public class SearchViewModel
    { 
        public ICollection<PostIndexViewModel> IndexPosts { get; set; }

        public string SearchedString { get; set; }
    }
}
