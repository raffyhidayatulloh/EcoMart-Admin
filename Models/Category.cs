using System.ComponentModel.DataAnnotations;

namespace EcoMart.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(255)]
        public string CategoryName { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime UpdatedDate { get; set; }
    }
}
