using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Catpuccino_FinalProject.Data; // Ensure this matches your Data folder
using Catpuccino_FinalProject.Models;

namespace Catpuccino_FinalProject.Controllers
{
    public class AdminController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly AppDbContext _context; // Added this back

        public AdminController(IWebHostEnvironment env, AppDbContext context)
        {
            _env = env;
            _context = context;
        }

        private const string AdminUsername = "admin";
        private const string AdminPassword = "admin123";

        // ── LOGIN ──
        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Login(AdminLoginModel model)
        {
            if (!ModelState.IsValid) return View(model);

            if (model.Username == AdminUsername && model.Password == AdminPassword)
            {
                HttpContext.Session.SetString("IsAdmin", "true");
                return RedirectToAction("Dashboard");
            }

            ModelState.AddModelError(string.Empty, "Invalid username or password.");
            return View(model);
        }

        // ── LOGOUT ──
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        // ── DASHBOARD ──
        public IActionResult Dashboard()
        {
            if (HttpContext.Session.GetString("IsAdmin") != "true")
                return RedirectToAction("Login");

            // Pulling real counts from the database
            ViewBag.TotalOrders = 5; // Replace with _context.Orders.Count() when table is ready
            ViewBag.PendingOrders = 5;
            ViewBag.CompletedOrders = 5;
            ViewBag.ProductCount = _context.Products.Count();
            ViewBag.RecentOrders = GetSampleOrders();
            return View();
        }

        // ── ORDERS ──
        public IActionResult Orders(string filter = "All")
        {
            if (HttpContext.Session.GetString("IsAdmin") != "true")
                return RedirectToAction("Login");

            ViewBag.Filter = filter;
            ViewBag.Orders = GetSampleOrders();
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult SaveOrderStatuses(string[] orderIds, string[] statuses)
        {
            TempData["Success"] = "Statuses updated.";
            return RedirectToAction("Orders");
        }

        // ── PRODUCTS ──
        public IActionResult Products()
        {
            if (HttpContext.Session.GetString("IsAdmin") != "true")
                return RedirectToAction("Login");

            var vm = new ProductsPageViewModel
            {
                // Pulling real products from CatpuccinoDb
                Products = _context.Products.Select(p => new AdminProductModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Category = p.Category,
                    Description = p.Description,
                    Price = p.Price,
                    ImageUrl = p.ImageUrl
                }).ToList(),
                ShowAddModal = TempData["ShowAddModal"]?.ToString() == "true"
            };

            return View(vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProduct(ProductsPageViewModel vm)
        {
            var model = vm.NewProduct;

            if (!ModelState.IsValid)
            {
                TempData["ShowAddModal"] = "true";
                return RedirectToAction("Products");
            }

            string imagePath = "/images/default-product.png";

            if (model.Photo != null && model.Photo.Length > 0)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "products");
                Directory.CreateDirectory(uploadsFolder);
                var fileName = Guid.NewGuid() + Path.GetExtension(model.Photo.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await model.Photo.CopyToAsync(stream);
                imagePath = "/uploads/products/" + fileName;
            }

            // Saving to Product Table
            var newProduct = new Product
            {
                Name = model.Name,
                Category = model.Category,
                Description = model.Description,
                Price = model.Price,
                ImageUrl = imagePath
            };

            _context.Products.Add(newProduct);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Product added successfully.";
            return RedirectToAction("Products");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Products");
        }

        private List<dynamic> GetSampleOrders() => new();
    }
}