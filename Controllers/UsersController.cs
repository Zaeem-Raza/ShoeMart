using Microsoft.AspNetCore.Mvc;
using ShoeMart.Models.Entities;
using ShoeMart.Models.Interfaces;
using ShoeMart.Models.Repositories;
namespace ShoeMart.Controllers
{
	public class UsersController : Controller
	{
		private IUsersRepository repository;

		public UsersController(IUsersRepository repo)
        {
            repository = repo;
        }

		public IActionResult Index()
		{
            // Checking if the user is logged in or not
            bool isUserLoggedIn = HttpContext.Request.Cookies.ContainsKey("User");
            if (!isUserLoggedIn)
            {
                return RedirectToAction("Login", "Users");
            }

			string cookie = HttpContext.Request.Cookies["User"];
            string[] data = cookie.Split('|');
            string Id = Convert.ToString(data[0]);

			// Get User Information
			Users user = repository.GetById(Id);

            return View(user);
		}

		[HttpPost]
		public IActionResult Index(UsersFormModal u)
		{
			var response = repository.ValidateFileAndSaveToFolder(u.Image);
			bool isImageSaved = response.result;
			string imagePath = response.imagePath;
			string finalImagePath = "/images/users/" + response.imagePath;

			if(!isImageSaved)
			{
				return View(u);
			}

			//// Store the image path in the db 
			Users user = new Users
			{
				Id = u.Id,
				UserName = u.UserName,
				Email = u.Email,
				PhoneNumber = u.PhoneNumber,
				Address = u.Address,
				Role = u.Role,
				Image = null
			};
			if (isImageSaved && imagePath != null)
			{
				user.Image = finalImagePath;
			}

			repository.Update(user);
			return RedirectToAction("Index", "Products");
		}

		// Login page 
		public IActionResult Login()
		{
			// Checking if the user is logged in or not
			bool isUserLoggedIn = HttpContext.Request.Cookies.ContainsKey("User");
			if(isUserLoggedIn)
			{
				return RedirectToAction("Index", "Products");
			}

			Users user = new Users { Email = string.Empty, PasswordHash = string.Empty };
			if (HttpContext.Request.Cookies.ContainsKey("LoginUser"))
			{
				string cookie = HttpContext.Request.Cookies["LoginUser"];
				string[] data = cookie.Split('|');
				user.Email = data[0];
				user.PasswordHash = data[1];
			}
			return View(user);
		}

		[HttpPost]
		public IActionResult Login(Users u)
		{
			Users user = repository.Login(u);
			if (user?.Id != null && user?.UserName != null && user?.Role != null)
			{
                // Login successful
                // Storing the id and role in the cookie
                string CookieValue = $"{user.Id}|{user.UserName}|{user.Role}";
                CookieOptions option = new CookieOptions();
                option.Expires = DateTime.Now.AddDays(1);
                HttpContext.Response.Cookies.Append("User", CookieValue, option);

                // remove cookie immediately
                if (HttpContext.Request.Cookies.ContainsKey("LoginUser"))
				{
					HttpContext.Response.Cookies.Delete("LoginUser");
				}
				return RedirectToAction("Index", "Products");
			}
			else
			{
				// login failed
				// generate a cookie to save state
				string CookieValue = $"{u.Email}|{u.PasswordHash}";
				CookieOptions option = new CookieOptions();
				option.Expires = DateTime.Now.AddDays(1);
				HttpContext.Response.Cookies.Append("LoginUser", CookieValue, option);
				ViewBag.Message = "Invalid UserName or Password";
				return View(u);
			}
		}

		public IActionResult SignUp()
		{
            // Checking if the user is logged in or not
            bool isUserLoggedIn = HttpContext.Request.Cookies.ContainsKey("User");
            if (isUserLoggedIn)
            {
                return RedirectToAction("Index", "Products");
            }

            // Checking the cookie
            bool isUserData = HttpContext.Request.Cookies.ContainsKey("SignUpUser");

			Users user = new Users { UserName = String.Empty, Email = String.Empty, PasswordHash = String.Empty, PhoneNumber = String.Empty, Address = String.Empty };
			if (isUserData)
			{
				string cookie = HttpContext.Request.Cookies["SignUpUser"];
				string[] info = cookie.Split('|');
				user.UserName = info[0];
				user.Email = info[1];
				user.PasswordHash = info[2];
				user.PhoneNumber = info[3];
				user.Address = info[4];
			}

			return View(user);
		}

		[HttpPost]
		public IActionResult SignUp(Users u)
		{
			string id = Guid.NewGuid().ToString();
			string role = "Customer";

			u.Id = id;
			u.Role = role;

			(bool result, string message) = repository.SignUp(u);
			ViewBag.Message = message;

			if (result)
			{
				// Remove cookie in this scenario
				if (HttpContext.Request.Cookies.ContainsKey("SignUpUser"))
				{
					HttpContext.Response.Cookies.Delete("SignUpUser");
				}
				return RedirectToAction("Login", "Users");
			}
			else
			{
				// Storing the form state
				string userData = $"{u.UserName}|{u.Email}|{u.PasswordHash}|{u.PhoneNumber}|{u.Address}";
				CookieOptions option = new CookieOptions();
				option.Expires = DateTime.Now.AddDays(1);
				HttpContext.Response.Cookies.Append("SignUpUser", userData, option);
			}
			return View(u);

		}
	
		public IActionResult Logout()
		{
			bool isUserLoggedIn = HttpContext.Request.Cookies.ContainsKey("User");
			if(isUserLoggedIn)
			{
				HttpContext.Response.Cookies.Delete("User");
			}

			return RedirectToAction("Index", "Home");
		}
	}
}