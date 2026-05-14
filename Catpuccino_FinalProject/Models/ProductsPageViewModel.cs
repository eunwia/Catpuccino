namespace Catpuccino_FinalProject.Models
{
    public class ProductsPageViewModel
    {
        public List<AdminProductModel> Products { get; set; } = new();
        public ProductViewModel NewProduct { get; set; } = new();
        public ProductViewModel EditProduct { get; set; } = new();
        public bool ShowAddModal { get; set; }
    }
}