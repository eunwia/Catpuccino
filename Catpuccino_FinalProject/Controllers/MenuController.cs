using Microsoft.AspNetCore.Mvc;

namespace Catpuccino_FinalProject.Controllers
{
    public class MenuController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Menus()
        {
            return View();
        }

    }
}
