using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace WebUı.Models
{
    public class AppUser : IdentityUser
    {
        [MaxLength(255)]
        public string? Name { get; set; }
        [MaxLength(255)]
        public string? Surname { get; set; }
    }
}
