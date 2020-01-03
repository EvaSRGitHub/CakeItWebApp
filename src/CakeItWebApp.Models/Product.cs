using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CakeItWebApp.Models
{
    public class Product
    {
        public Product()
        {
            this.IsDeleted = false;
            this.Orders = new HashSet<OrderProduct>();
        }

        public int Id { get; set;}

        [Required]
        public string Name { get; set; }

        [ForeignKey("Category"), Required]
        public int CategoryId { get; set; }

        public virtual Category Category { get; set; }

        [Required]
        public string Image { get; set; }

        [Required]
        [Range(typeof(decimal), "0.01", "79228162514264337593543950335", ErrorMessage = "Price should be positive value grater than 0.")]
        public decimal Price { get; set; }

        public string Description { get; set; }

        [Required]
        public bool IsDeleted { get; set; }

        [JsonIgnore]
        public virtual Ingredients Ingredients { get; set; }

        public double? Rating { get; set; }

        public int? RatingVotes { get; set; }

        [JsonIgnore]
        public virtual ICollection<OrderProduct> Orders { get; set; }
    }
}
