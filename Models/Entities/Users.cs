using Microsoft.AspNetCore.Identity;

namespace ShoeMart.Models.Entities
{
    public class Users:IdentityUser
    {
 
        public string Address { get; set; }
        public string Role { get; set; }
        public string Image { get; set; }
    }
}
