using Microsoft.AspNetCore.Mvc;
using Catpuccino_FinalProject.Data;
using Catpuccino_FinalProject.Models;

namespace Catpuccino_FinalProject.Controllers
{
    public class UserController : Controller
    {
        private readonly AppDbContext _context;

        // Dependency Injection
        public UserController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Register() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                
                // Save to database
                _context.Users.Add(user);
                _context.SaveChanges();

                // Go to login page after successful registration
                return RedirectToAction("Login");
            }
            return View(user);
        }

        public IActionResult Login() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(UserLoginModel loginData)
        {
            if (ModelState.IsValid)
            {
                // Check if user exists in the database
                var user = _context.Users.FirstOrDefault(u =>
                    u.Username == loginData.Username && u.Password == loginData.Password);

                if (user != null)
                {
                    // Redirect to a home page or dashboard
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "Invalid Username or Password.");
            }
            return View();
        }
    }
}