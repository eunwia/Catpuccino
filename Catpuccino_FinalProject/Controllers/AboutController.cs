using Microsoft.AspNetCore.Mvc;

namespace Catpuccino_FinalProject.Controllers
{
    public class AboutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult AboutUs()
        {
            return View();
        }
    }
}
