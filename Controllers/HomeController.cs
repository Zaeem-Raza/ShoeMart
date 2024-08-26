using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoeMart.Models;
using ShoeMart.Models.Entities;
using ShoeMart.Models.Interfaces;
using ShoeMart3.Models;
using System.Diagnostics;

namespace ShoeMart.Controllers
{
    public class HomeController : Controller
    {
        // Landing / Home Page action method
        private IProductRepository repo;

        public HomeController(IProductRepository _repo)
        {
            repo = _repo;
        }

        public IActionResult Index()
        {
            IEnumerable<Products> products = repo.GetFeaturedItems();
            return View(products);
        }

        // By default
        [Authorize]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


    }
}
