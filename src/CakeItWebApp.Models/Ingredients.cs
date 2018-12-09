using CakeItWebApp.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CakeItWebApp.Models
{
    public class Ingredients
    {
        public int Id { get; set; }

        [Required]
        public SpongeType Sponge { get; set; }

        [Required]
        public CreamType FirstLayerCream { get; set; }

        [Required]
        public CreamType SecondLayerCream { get; set; }

        [Required]
        public FillingType Filling { get; set; }

        [Required]
        public SideDecorationType SideDecoration { get; set; }

        [Required]
        public TopDecorationType TopDecoration { get; set; }

        public int ProductId { get; set; }

        public virtual Product Product { get; set; }
    }
}
