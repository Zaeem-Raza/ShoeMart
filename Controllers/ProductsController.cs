using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using ShoeMart.Models.Entities;
using ShoeMart.Models.Interfaces;
using ShoeMart.Models.Repositories;
using System.Globalization;

namespace ShoeMart.Controllers
{
    [Authorize]
    public class ProductsController : Controller
    {
        private IProductRepository repo;
        private readonly UserManager<Users> _userManager;

        public ProductsController(IProductRepository _repo,UserManager<Users> userManager)
        {
            _userManager = userManager;
            repo = _repo;

        }

        
        public async Task<IActionResult> IndexAsync(string[] category, string[] color, string[] size, string search)
        {
            // Apply filters if any are provided
            IEnumerable<Products> products = repo.GetAll();
            if ((category != null && category.Length > 0) ||
                (color != null && color.Length > 0) ||
                (size != null && size.Length > 0))
            {
                products = products.Where(p =>
                    (category != null && category.Length > 0 && category.Contains(p.Category)) ||
                    (color != null && color.Length > 0 && color.Contains(p.Color)) ||
                    (size != null && size.Length > 0 && size.Contains(p.Size))
                );
            }

            // Apply search filter if a search value is provided
            if (!string.IsNullOrEmpty(search))
            {
                products = products.Where(p =>
                    p.Name.Contains(search, StringComparison.OrdinalIgnoreCase)
                );
            }

            // Extracting the User Image
            var user=await _userManager.GetUserAsync(User);

            UsersRepository u = new UsersRepository();
            string image = user.Image;
            if (string.IsNullOrEmpty(image))
            {
                image = "/images/user.png";
            }
            ViewBag.Image = image;
            ViewBag.Name = user.UserName;
            ViewBag.Role = user.Role;


            // Check the current number of items in the cart (if any)
            int CartItems = HttpContext.Session.Keys.Count();
            ViewBag.CartItems = CartItems;


            return View(products);
        }

        
        [HttpPost]
        public IActionResult Index(string[] category, string[] color, string[] size)
        {
            if (category != null)
            {
                foreach (var item in category)
                {
                    Console.WriteLine($"Category: {item}");
                }
            }
            if (color != null)
            {
                foreach (var item in color)
                {
                    Console.WriteLine($"Color: {item}");
                }
            }
            if (size != null)
            {
                foreach (var item in size)
                {
                    Console.WriteLine($"Size: {item}");
                }
            }

            return View();
        }

        
        public IActionResult ProductDetails(string id)
        {
            Products product = repo.GetById(id);
            return View(product);
        }
        
        public IActionResult Add() {

            return View();
        }

        
        public IActionResult Load()
        {
            repo.LoadProducts();
            return RedirectToAction("Index", "Products");
        }

        
        [HttpPost]
        public IActionResult Add(ProductsFormModal p)
        {
            // Adding unique id to the product
            string id = Guid.NewGuid().ToString();
            p.Id = id;

            // Concatenating colors
            string color = String.Join('|', p.Color);

            // Validating and saving the product image to the file
            var response = repo.ValidateFileAndSaveToProductsFolder(p.Image);
            bool isImageSaved = response.result;
            string imagePath = response.imagePath;
            string finalImagePath = "/images/products/" + response.imagePath;

            if (!isImageSaved)
            {
                return View(p);
            }

            // Store product in the database
            Products product = new Products
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Size = p.Size,
                Category = p.Category,
                Stock = p.Stock,
                Description = p.Description,
                Image = finalImagePath,
                Color = color
            };

            repo.Add(product);
            return RedirectToAction("Index", "Products");
        }

        
        public IActionResult Delete(string id)
        {
            // Deleting the product with given id
            bool isDeleted = repo.Delete(id);
            if(isDeleted)
            {
                return Json(new { result = true });
            }
            else
            {
                return Json(new { result = false });
            }
        }

        
		public IActionResult Update(string id)
		{
            Products product = repo.GetById(id);
            return View(product);  
		}
        
       
        [HttpPost]
        public IActionResult Update(ProductsFormModal p)
        {
            // Concatenating colors
            string color = String.Join('|', p.Color);

            // Validating and saving the product image to the file
            var response = repo.ValidateFileAndSaveToProductsFolder(p.Image);
            bool isImageSaved = response.result;
            string imagePath = response.imagePath;
            string finalImagePath = "/images/products/" + response.imagePath;

            if (!isImageSaved)
            {
                return View(p);
            }

            // Update product in the database
            Products product = new Products
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Size = p.Size,
                Category = p.Category,
                Stock = p.Stock,
                Description = p.Description,
                Image = finalImagePath,
                Color = color
            };
           
            repo.Update(product);
            return RedirectToAction("Index", "Products");
        }

       
        public  IActionResult GetCount()
        {
            IEnumerable<Products> products = repo.GetAll();
            int count = products.Count();
            return Json(new { result = count });
        }

        
        public async Task<IActionResult> GetFilteredProducts(string[] category, string[] color, string[] size, string search)
        {
            // Checking if the user is logged in or not
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Users");
            }

            // Extracting user information
            string name = user.UserName;
            string role = user.Role;
            string image = user.Image;

            if (string.IsNullOrEmpty(image))
            {
                image = "/images/user.png";
            }

            ViewBag.Name = name;
            ViewBag.Role = role;
            ViewBag.Image = image;

            // Apply filters if any are provided
            IEnumerable<Products> products = repo.GetAll();
            if ((category != null && category.Length > 0) ||
                (color != null && color.Length > 0) ||
                (size != null && size.Length > 0))
            {
                products = products.Where(p =>
                    (category != null && category.Length > 0 && category.Contains(p.Category)) ||
                    (color != null && color.Length > 0 && color.Contains(p.Color)) ||
                    (size != null && size.Length > 0 && size.Contains(p.Size))
                );
            }

            // Apply search filter if a search value is provided
            if (!string.IsNullOrEmpty(search))
            {
                products = products.Where(p =>
                    p.Name.Contains(search, StringComparison.OrdinalIgnoreCase)
                );
            }

            return PartialView("_ProductsListView", products);
        }

        public IActionResult GetFilteredProductsCount(string[] category, string[] color, string[] size, string search)
        {

            IEnumerable<Products> products = repo.GetAll();

            // Apply filters if any are provided
            if ((category != null && category.Length > 0) ||
                (color != null && color.Length > 0) ||
                (size != null && size.Length > 0))
            {
                products = products.Where(p =>
                    (category != null && category.Length > 0 && category.Contains(p.Category)) ||
                    (color != null && color.Length > 0 && color.Contains(p.Color)) ||
                    (size != null && size.Length > 0 && size.Contains(p.Size))
                );
            }

            // Apply search filter if a search value is provided
            if (!string.IsNullOrEmpty(search))
            {
                products = products.Where(p =>
                    p.Name.Contains(search, StringComparison.OrdinalIgnoreCase)
                );
            }

            int count = products.Count();
            Console.WriteLine("Count: " + count);
            return Json(new { result = count });
        }

        public IActionResult AddToCart(string id)
        {
            Products product = repo.GetById(id);

            // Check whether this product already stored in session,
            // if it is present, then increment the current count
            string keyName = $"Product-{product.Id}";
            string isProductPresent = HttpContext.Session.GetString(keyName);

            if(isProductPresent != null)
            {
                var attributes = isProductPresent.Split('|');

                // Update attributes with new values
                int incCurrentCount = Convert.ToInt32(attributes[5]) + 1;
                attributes[5] = Convert.ToString(incCurrentCount);

                string updatedProductString = string.Join('|', attributes);
                HttpContext.Session.SetString(keyName, updatedProductString);
            }
            else
            {
                string stringifiedProduct = $"{product.Id}|{product.Name}|{product.Stock}|{product.Price}|{product.Image}|{1}";
                HttpContext.Session.SetString(keyName, stringifiedProduct);
            }

            int count = HttpContext.Session.Keys.Count();
            return Json(new { result = true, totalItems = count });
        }

        public IActionResult RemoveFromCart(string id)
        {
            // get product from the session and decrement the current quantity 
            // if the quantity becomes 0, remove from the session keys
            
            string keyName= $"Product-{id}";
            string productString = HttpContext.Session.GetString(keyName);
            if (productString != null)
            {
                // get attributes of the product
                var attributes = productString.Split('|');
                int currentCount = Convert.ToInt32(attributes[5]);
                currentCount--;
                // if the current count is 0, remove the product from the session
                if (currentCount == 0)
                {
                    HttpContext.Session.Remove(keyName);
                }
                else
                {
                    // Update the current count
                    attributes[5] = Convert.ToString(currentCount);
                    string updatedProductString = string.Join('|', attributes);
                    HttpContext.Session.SetString(keyName, updatedProductString);
                }
            }
            return Json(true);
        }
    }
}
