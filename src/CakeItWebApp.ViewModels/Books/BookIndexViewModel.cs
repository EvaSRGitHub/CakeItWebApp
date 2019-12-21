using System.ComponentModel.DataAnnotations;

namespace CakeItWebApp.ViewModels.Books
{
    public class BookIndexViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Author { get; set; }

        [Required]
        public int Pages { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Download Url")]
        public string DownloadUrl { get; set; }

        [Required]
        [Display(Name = "Cover Image Url")]
        public string CoverUrl { get; set; }

        public double Rating { get; set; }

        public int RatingVotes { get; set; }
    }
}
