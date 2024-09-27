using EcoMart.Models;

namespace EcoMart.ViewModels
{
    public class ProductViewModel
    {
        public IQueryable<Product> Products { get; set; }
        public List<Category> Categories { get; set; }
        public string Term { get; set; }
        public string FilterBy { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public CreateProductViewModel CreateProductVM { get; set; }
        public EditProductViewModel EditProductVM { get; set; }
    }
}
