using System.ComponentModel.DataAnnotations;

namespace CakeItWebApp.ViewModels.CustomCake
{
    public class CustomCakeImgViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Side { get; set; }

        [Required]
        public string Top { get; set; }

        [Required]
        public string Img { get; set; }

        [Display(Name = "Is Deleted")]
        public bool IsDeleted { get; set; }
    }
}
