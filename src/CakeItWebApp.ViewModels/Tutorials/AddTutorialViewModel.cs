using System.ComponentModel.DataAnnotations;

namespace CakeItWebApp.ViewModels.Tutorials
{
    public class AddTutorialViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Url { get; set; }
    }
}
