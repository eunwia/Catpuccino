using Microsoft.AspNetCore.Mvc;
using Catpuccino_FinalProject.Data;
using Catpuccino_FinalProject.Models;

namespace Catpuccino_FinalProject.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Profile
        public IActionResult Profile()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "User");

            var user = _context.Users.FirstOrDefault(u => u.Id == userId);

            if (user == null)
                return RedirectToAction("Login", "User");

            return View(user);
        }

        // POST: Edit Profile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(User updatedUser, IFormFile imageFile)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "User");

            var userInDb = _context.Users.FirstOrDefault(u => u.Id == userId);

            if (userInDb == null)
                return RedirectToAction("Login", "User");

            // 1. Update profile image
            if (imageFile != null && imageFile.Length > 0)
            {
                string fileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
                string filePath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot/images/userpics",
                    fileName
                );

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                userInDb.ProfilePicture = fileName;
            }

            // 2. Update username & email
            userInDb.Username = updatedUser.Username;
            userInDb.Email = updatedUser.Email;

            // 3. Update password ONLY if not empty
            if (!string.IsNullOrWhiteSpace(updatedUser.Password))
            {
                userInDb.Password = updatedUser.Password;
            }

            _context.SaveChanges();

            // 4. Update session values
            HttpContext.Session.SetString("Username", userInDb.Username);
            HttpContext.Session.SetString("Email", userInDb.Email);

            return RedirectToAction("Profile");
        }
    }
}