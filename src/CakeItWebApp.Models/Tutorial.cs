using System.ComponentModel.DataAnnotations;

namespace CakeItWebApp.Models
{
    public class Tutorial
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        public string Url { get; set; }

        public double? Rating { get; set; }

        public int? RatingVotes { get; set; }
    }
}
