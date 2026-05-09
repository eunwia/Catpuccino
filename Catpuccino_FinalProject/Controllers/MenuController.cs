using Microsoft.AspNetCore.Mvc;
using Catpuccino_FinalProject.Data;
using Catpuccino_FinalProject.Models;
using System.Linq;

namespace Catpuccino_FinalProject.Controllers
{
    public class MenuController : Controller
    {
        private readonly AppDbContext _context;

        // Inject the database context
        public MenuController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Menus()
        {
            // Fetch all products from the database and pass them to the view
            var products = _context.Products.ToList();
            return View(products);
        }
    }
}