using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoeMart.Models.Entities;

namespace ShoeMart.Controllers
{
    public class ShoppingCartController : Controller
    {
        [Authorize]
        public IActionResult Index()
        {
            // get items from the sessions 
            // there may be multiple items in the cart
            List<CartItem> cartItems = new List<CartItem>();
            
            // iterate over the session keys and store it in the list
            foreach (var key in HttpContext.Session.Keys)
            {
                string value = HttpContext.Session.GetString(key);
                string[] items = value.Split("|");
                CartItem cartItem = new CartItem();
                cartItem.Id=items[0];
                cartItem.Name=items[1];
                cartItem.Stock=Convert.ToInt32(items[2]);
                cartItem.Price=Convert.ToDecimal(items[3]);
                cartItem.Image=items[4];
                cartItem.CurrentCount=Convert.ToInt32(items[5]);
                cartItems.Add(cartItem);
            }

            return View(cartItems);
        }
        [Authorize]
        public IActionResult Payment()
        {
            return View();
        }
    }
}
