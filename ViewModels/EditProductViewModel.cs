using EcoMart.Models;

namespace EcoMart.ViewModels
{
    public class EditProductViewModel
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public int StockQuantity { get; set; }
        public int CategoryId { get; set; }
        public List<Category>? Categories { get; set; }
        public string ImageUrl { get; set; }
        public IFormFile? Image { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
