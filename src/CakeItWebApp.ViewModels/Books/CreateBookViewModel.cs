using System.ComponentModel.DataAnnotations;

namespace CakeItWebApp.ViewModels.Books
{
    public class CreateBookViewModel
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Author { get; set; }

        [Required]
        public int Pages { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string DownloadUrl { get; set; }

        [Required]
        public string CoverUrl { get; set; }
    }
}
