using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace MovieManagement.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime RegistrationDate { get; set; } = DateTime.Now;

        public ICollection<Review> Reviews { get; set; } = new List<Review>();
       
    }
}