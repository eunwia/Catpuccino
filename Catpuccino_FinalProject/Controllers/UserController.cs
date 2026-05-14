using Microsoft.AspNetCore.Mvc;
using Catpuccino_FinalProject.Data;
using Catpuccino_FinalProject.Models;
using System.Linq;

namespace Catpuccino_FinalProject.Controllers
{
    public class UserController : Controller
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Register() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(UserRegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // 2. Create a NEW User object 
                var newUser = new User
                {
                    Username = model.Username,
                    Email = model.Email,
                    Password = model.Password // Values from the form go here
                };

                // 3. Add the ENTITY to the database
                _context.Users.Add(newUser);
                _context.SaveChanges();

                return RedirectToAction("Login");
            }

            // 4. If validation (like ConfirmPassword) fails, return the model back to the view
            return View(model);
        }
        public IActionResult Login() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(UserLoginModel loginData)
        {
            if (!ModelState.IsValid)
                return View(loginData);

            var user = _context.Users.FirstOrDefault(u =>
                 u.Username == loginData.Username &&
                 u.Email == loginData.Email &&
                 u.Password == loginData.Password
             );

            if (user == null)
            {
                ModelState.AddModelError("", "Invalid credentials.");
                return View(loginData);
            }

            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("Username", user.Username);
            HttpContext.Session.SetString("Email", user.Email);

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}