using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShoeMart.Models.Entities;
using ShoeMart.Models.Interfaces;
using ShoeMart.Models.Repositories;
namespace ShoeMart.Controllers
{
	public class UsersController : Controller
	{
		private IUsersRepository repository;
        private readonly UserManager<Users> _userManager;
        public UsersController(IUsersRepository repo,UserManager<Users> user)
        {
            repository = repo;
			_userManager = user;
        }
		[Authorize]
		public async Task<IActionResult> Index()
		{
			// if the user is logged in, pass user in the view
			// if not,redirect to login page
			var user =await _userManager.GetUserAsync(HttpContext.User);
			if(user == null)
            {
                return RedirectToAction("Login", "Users");
            }
			return View(user);
		}

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Index(UsersFormModal u)
        {
            // Retrieve the logged-in user
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user == null)
            {
                return RedirectToAction("Login", "Users");
            }

            // Validate and save the image
            var response = repository.ValidateFileAndSaveToFolder(u.Image);
            bool isImageSaved = response.result;
            string imagePath = response.imagePath;
            string finalImagePath = "/images/users/" + response.imagePath;

            if (!isImageSaved)
            {
                // If the image isn't saved, map the UsersFormModal to Users and return the view
                var usersModel = new Users
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    Address = u.Address,
                    Role = u.Role,
                    Image = user.Image // Maintain the existing image if saving fails
                };
                return View(usersModel);
            }

            // Update the user properties with the form data
            user.UserName = u.UserName;
            user.Email = u.Email;
            user.PhoneNumber = u.PhoneNumber;
            user.Address = u.Address;
            user.Role = u.Role; // This might need to be handled differently based on your roles setup

            if (isImageSaved && imagePath != null)
            {
                user.Image = finalImagePath;
            }

            // Update the user in the database using UserManager
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
				Console.WriteLine("updated user");
                // Redirect to the Products index page after successful update
                return RedirectToAction("Index", "Products");
            }

            // If the update fails, add errors to the ModelState and return the view
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(user); 
        }
       public IActionResult Logout()
        {
            // delete all cookies
            for (int i = 0; i < Request.Cookies.Count; i++)
            {
                Response.Cookies.Delete(Request.Cookies.Keys.ElementAt(i));
            }
            return RedirectToAction("Index", "Home");
        }
    }
}