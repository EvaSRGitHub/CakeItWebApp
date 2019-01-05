using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace CakeItWebApp.Models
{
    // Add profile data for application users by adding properties to the CakeItUser class
    public class CakeItUser : IdentityUser
    {
        public CakeItUser()
        {
            this.IsDeleted = false;
            this.CreatedOn = DateTime.UtcNow;
            this.Orders = new HashSet<Order>();
        }

        public DateTime CreatedOn { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
    }
}
