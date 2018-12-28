﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CakeItWebApp.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Author { get; set; }

        [Required]
        public int Pages { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string DownloadUrl { get; set; }

        [Required]
        public string CoverUrl { get; set; } 

        public double? Rating { get; set; }

        public int? RatingVotes { get; set; }
    }
}
