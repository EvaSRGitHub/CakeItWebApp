using CakeItWebApp.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CakeItWebApp.Models
{
    public class Product
    {
        public Product()
        {
            this.IsDeleted = false;
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
        public decimal Price { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public bool IsDeleted { get; set; }

        public virtual Ingredients Ingredients { get; set; }

        public double? Rating { get; set; }

        public int? RatingVotes { get; set; }
    }
}
