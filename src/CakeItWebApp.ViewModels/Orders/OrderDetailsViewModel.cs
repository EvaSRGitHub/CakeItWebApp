using System.ComponentModel.DataAnnotations;

namespace CakeItWebApp.ViewModels.Orders
{
    public class OrderDetailsViewModel
    {
        [Required]
        public int OrderId { get; set; }

        [Required(ErrorMessage = "Please enter your name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please enter your last name")]
        [MinLength(3, ErrorMessage = "Last name should be at least 3 letters.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Please enter your email")]
        [RegularExpression(@"^[\w!#$%&'*+\-\/=?\^_`{|}~]+(\.[\w!#$%&'*+\-\/=?\^_`{|}~]+)*@((([\-\w]+\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,3}))$", ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter your phone number")]
        [RegularExpression(@"^\+?(\d+)\/(\d{9})$", ErrorMessage = "Invalid Phone number format.")]
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
