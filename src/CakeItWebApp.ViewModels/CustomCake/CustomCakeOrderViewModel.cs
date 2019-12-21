using System.ComponentModel.DataAnnotations;

namespace CakeItWebApp.ViewModels.CustomCake
{
    public class CustomCakeOrderViewModel
    {
        private const decimal SlicePrice = 2.75m;

        public int Id { get; set; }

        [Required]
        public string Sponge { get; set; }

        [Required]
        public string FirstLayerCream { get; set; }

        [Required]
        public string SecondLayerCream { get; set; }

        [Required]
        public string Filling { get; set; }

        [Required]
        public string SideDecoration { get; set; }

        [Required]
        public string TopDecoration { get; set; }

        [Required]
        public int NumberOfSlices { get; set; }

        public decimal Price => this.NumberOfSlices * SlicePrice;

        public string Img { get; set; }
    }
}
