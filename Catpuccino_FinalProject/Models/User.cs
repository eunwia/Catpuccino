using System.ComponentModel.DataAnnotations;

namespace Catpuccino_FinalProject.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string? Email { get; set; } // Added back for the profile

        public string? ProfilePicture { get; set; }
    }
}