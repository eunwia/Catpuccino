using Microsoft.AspNetCore.Mvc;

namespace Catpuccino_FinalProject.Controllers
{
    public class OrderController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Order()
        {
            return View();
        }
    }
}
