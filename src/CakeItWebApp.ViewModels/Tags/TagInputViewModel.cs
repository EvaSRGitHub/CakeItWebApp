using System.ComponentModel.DataAnnotations;

namespace CakeItWebApp.ViewModels.Tags
{
    public class TagInputViewModel
    {
        public TagInputViewModel()
        {
            this.IsDeleted = false;
        }
        
        public int Id { get; set; }

        [Required, MinLength(3)]
        public string Name { get; set; }

        [Required]
        public bool IsDeleted { get; set; }
    }
}
