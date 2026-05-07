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

        // GET: Show Profile
        public IActionResult Profile()
        {
            // ✅ Read logged-in user's ID from session
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "User");

            var user = _context.Users.FirstOrDefault(u => u.Id == userId);

            if (user == null)
                return RedirectToAction("Login", "User");

            return View(user);
        }

        // POST: Update Profile
        [HttpPost]
        public async Task<IActionResult> EditProfile(User updatedUser, IFormFile imageFile)
        {
            // ✅ Use session ID instead of submitted Id to prevent tampering
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "User");

            var userInDb = _context.Users.Find(userId);

            if (userInDb != null)
            {
                // 1. Handle Image Upload
                if (imageFile != null && imageFile.Length > 0)
                {
                    string fileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/userpics", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }

                    userInDb.ProfilePicture = fileName;
                }

                // 2. Update other fields
                userInDb.Username = updatedUser.Username;
                userInDb.Email = updatedUser.Email;

                _context.SaveChanges();

                // ✅ Update session username in case it changed
                HttpContext.Session.SetString("Username", userInDb.Username);
            }

            return RedirectToAction("Profile");
        }
    }
}