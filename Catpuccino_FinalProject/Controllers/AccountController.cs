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
            var user = _context.Users.FirstOrDefault(); // Just get the first user available

            if (user == null)
            {
                // Create a fake user just so the page doesn't crash
                user = new User { Username = "Guest", Email = "guest@cafe.com" };
            }

            return View(user);
        }

        // POST: Update Profile
        [HttpPost]
        public async Task<IActionResult> EditProfile(User updatedUser, IFormFile imageFile)
        {
            var userInDb = _context.Users.Find(updatedUser.Id);
            if (userInDb != null)
            {
                // 1. Handle the Image Upload
                if (imageFile != null && imageFile.Length > 0)
                {
                    // Create a unique filename
                    string fileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/userpics", fileName);

                    // Save the file to the folder
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }

                    // Save the filename to the database
                    userInDb.ProfilePicture = fileName;
                }

                // 2. Update other fields
                userInDb.Username = updatedUser.Username;
                userInDb.Email = updatedUser.Email;

                _context.SaveChanges();
            }
            return RedirectToAction("Profile");
        }
    }
}