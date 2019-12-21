using CakeItWebApp.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace CakeItWebApp.Models
{
    public class Category
    { 
        public int Id { get; set; }

        [Required]
        public CategoryType Type { get; set; }
    }
}
