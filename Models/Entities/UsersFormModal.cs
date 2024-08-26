using Microsoft.AspNetCore.Identity;
namespace ShoeMart.Models.Entities
{
    public class UsersFormModal:IdentityUser
    {
        
        public string Address { get; set; }
        public string Role { get; set; }
        public IFormFile Image { get; set; }
    }
}
