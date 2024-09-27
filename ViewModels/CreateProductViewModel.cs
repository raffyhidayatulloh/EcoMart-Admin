namespace EcoMart.ViewModels
{
    public class CreateProductViewModel
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public int StockQuantity { get; set; }
        public int CategoryId { get; set; }
        public IFormFile ImageUrl { get; set; }
    }
}
