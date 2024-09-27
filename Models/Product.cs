using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcoMart.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(255)]
        public string ProductName { get; set; }

        public string Description { get; set; }

        public int Price { get; set; }

        public int StockQuantity { get; set; }

        [ForeignKey("Category")]
        public int CategoryId { get; set; }

        public Category? Category { get; set; }

        public string ImageUrl { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime UpdatedDate { get; set; }
    }
}
