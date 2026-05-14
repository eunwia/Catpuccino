using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Catpuccino_FinalProject.Models
{
    public class ProductViewModel
    {
        [Required(ErrorMessage = "Product name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Enter a valid price")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public string Category { get; set; }

        [Required(ErrorMessage = "Description is required")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Photo is required")]
        public IFormFile? Photo { get; set; } 
    }
}