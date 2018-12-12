using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CakeItWebApp.ViewModels.ShoppingCart
{
    public class CheckoutViewModel
    {
        [Required(ErrorMessage = "Please enter your name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please enter your last name")]
        [MinLength(3, ErrorMessage = "Last name should be at least 3 letters.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Please enter your email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter your phone number")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Please enter delivery country name")]
        [MinLength(3, ErrorMessage = "Cuountry name should be at least 3 letters.")]
        public string Country { get; set; }

        [Required(ErrorMessage = "Please enter delivery city name")]
        [MinLength(3, ErrorMessage = "City name should be at least 3 letters.")]
        public string City { get; set; }

        [Required(ErrorMessage = "Please enter delivery address")]
        [MinLength(3, ErrorMessage = "Address name should be at least 3 letters.")]
        public string Address { get; set; }
    }
}
