using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace ShoeMart.Controllers
{
    public class ErrorsController : Controller
    {
        [HttpGet("/Errors/404")]
        public IActionResult NotFound()
        {
            return View();
        }

        [HttpGet("/Errors/500")]
        public IActionResult InternalServerError()
        {
            return View();
        }
    }
}
