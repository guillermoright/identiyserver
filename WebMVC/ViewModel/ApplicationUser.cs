using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebMVC.ViewModel
{
    public class ApplicationUser : IdentityUser
    {
        
        public string Expiration { get; set; }
    
        [Required]
        public string Name { get; set; }
        [Required]
        public string LastName { get; set; }
    }
}
