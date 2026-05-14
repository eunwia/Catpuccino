using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Catpuccino_FinalProject.Data;
using Catpuccino_FinalProject.Models;

namespace Catpuccino_FinalProject.Controllers
{
    public class AdminController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly AppDbContext _context;

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

            var today = DateTime.Today;

            var todaysOrders = _context.Orders
                .Where(o => o.Date.Date == today)
                .Include(o => o.Items)
                .ToList();

            ViewBag.TotalOrders = todaysOrders.Count;
            ViewBag.PendingOrders = todaysOrders.Count(o => o.Status == "Pending");
            ViewBag.CompletedOrders = todaysOrders.Count(o => o.Status == "Done");
            ViewBag.ProductCount = _context.Products.Count();

            ViewBag.RecentOrders = _context.Orders
                .Include(o => o.Items)
                .OrderByDescending(o => o.Id)
                .Take(10)
                .ToList();

            return View();
        }

        // ── ORDERS ──
        public IActionResult Orders(string filter = "All")
        {
            if (HttpContext.Session.GetString("IsAdmin") != "true")
                return RedirectToAction("Login");

            var query = _context.Orders.Include(o => o.Items).AsQueryable();

            if (filter == "Pending")
                query = query.Where(o => o.Status == "Pending");
            else if (filter == "Done")
                query = query.Where(o => o.Status == "Done");

            ViewBag.Filter = filter;
            ViewBag.Orders = query.OrderByDescending(o => o.Id).ToList();

            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult SaveOrderStatuses(int[] orderIds, string[] statuses)
        {
            for (int i = 0; i < orderIds.Length; i++)
            {
                var order = _context.Orders.Find(orderIds[i]);
                if (order != null)
                {
                    order.Status = statuses[i];
                }
            }
            _context.SaveChanges();

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

        // ── ADD PRODUCT ──
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProduct([Bind(Prefix = "NewProduct")] ProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var vm = new ProductsPageViewModel
                {
                    Products = _context.Products.Select(p => new AdminProductModel
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Category = p.Category,
                        Description = p.Description,
                        Price = p.Price,
                        ImageUrl = p.ImageUrl
                    }).ToList(),
                    NewProduct = model,
                    ShowAddModal = true
                };

                return View("Products", vm);
            }

            string imagePath = "/images/default-product.png";

            if (model.Photo != null && model.Photo.Length > 0)
            {
                imagePath = await SaveImage(model.Photo);
            }

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

        // ── EDIT PRODUCT ──
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProduct(int Id, string Name, decimal Price, string Category, string Description, IFormFile? Photo)
        {
            if (Price < 0.01m)
            {
                TempData["EditPriceError"] = "Enter a valid price";
                TempData["EditId"] = Id.ToString();
                TempData["EditName"] = Name;
                TempData["EditPrice"] = Price.ToString();
                TempData["EditCategory"] = Category;
                TempData["EditDescription"] = Description;
                return RedirectToAction("Products");
            }

            var product = await _context.Products.FindAsync(Id);
            if (product == null) return NotFound();

            product.Name = Name;
            product.Price = Price;
            product.Category = Category;
            product.Description = Description;

            if (Photo != null && Photo.Length > 0)
            {
                product.ImageUrl = await SaveImage(Photo);
            }

            _context.Products.Update(product);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Product updated successfully.";
            return RedirectToAction("Products");
        }

        // ── DELETE PRODUCT ──
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

        // ── HELPER ──
        private async Task<string> SaveImage(IFormFile photo)
        {
            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "products");
            Directory.CreateDirectory(uploadsFolder);
            var fileName = Guid.NewGuid() + Path.GetExtension(photo.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await photo.CopyToAsync(stream);
            return "/uploads/products/" + fileName;
        }
    }
}