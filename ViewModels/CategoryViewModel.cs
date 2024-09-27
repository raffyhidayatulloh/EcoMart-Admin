using EcoMart.Models;

namespace EcoMart.ViewModels
{
    public class CategoryViewModel
    {
        public IQueryable<CategoryProductCountList> Categories { get; set; }
        public string Term { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public CreateCategoryViewModel CreateCategoryVM { get; set; }
        public EditCategoryViewModel EditCategoryVM { get; set; }
    }
}
