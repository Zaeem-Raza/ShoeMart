using ShoeMart.Models.Entities;
using System.Globalization;

namespace ShoeMart.Models.Interfaces
{
    public interface IUsersRepository : IRepository<Users>
    {
        public Users Login(Users user);

        public (bool result, string message) SignUp(Users user);

        public (bool result, string imagePath) ValidateFileAndSaveToFolder(IFormFile file);

        public string GetUserProfileImage(string Id);
    }
}
