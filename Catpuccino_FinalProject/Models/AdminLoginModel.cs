using System.ComponentModel.DataAnnotations;

namespace Catpuccino_FinalProject.Models
{
    public class AdminLoginModel
    {
        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

    }
}
