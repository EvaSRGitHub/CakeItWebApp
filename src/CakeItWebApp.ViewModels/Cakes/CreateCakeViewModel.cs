using System.ComponentModel.DataAnnotations;

namespace CakeItWebApp.ViewModels.Cakes
{
    public class CreateCakeViewModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Category Id")]
        public int CategoryId { get; set; }

        [Required]
        public string Image { get; set; }

        [Required]
        [Range(typeof(decimal), "0.01", "79228162514264337593543950335", ErrorMessage = "Price should be positive value grater than 0.")]
        public decimal Price { get; set; }

        [Required]
        public string Description { get; set; }
    }
}
