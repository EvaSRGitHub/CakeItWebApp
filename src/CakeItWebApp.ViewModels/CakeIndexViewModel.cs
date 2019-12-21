using CakeItWebApp.Models;
using System.ComponentModel.DataAnnotations;

namespace CakeItWebApp.ViewModels
{
    public class CakeIndexViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Image { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public string Description { get; set; }

        public double Rating { get; set; }

        public int RatingVotes { get; set; }

        public int CategoryId { get; set; }

        public virtual Category  Category {get; set;}

        public bool IsDeleted { get; set; }
    }
}
