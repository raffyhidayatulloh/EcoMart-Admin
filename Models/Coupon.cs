using System.ComponentModel.DataAnnotations;

namespace EcoMart.Models
{
    public class Coupon
    {
        [Key]
        public int Id { get; set; }

        public int DiscountPersent { get; set; }

        public int MaxDiscount { get; set; }

        public int MinPurchase { get; set; }

        public bool IsActive { get; set; }

        public DateTime ExpiryDate { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
