using EcoMart.Data.Enum;
using System.ComponentModel.DataAnnotations;

namespace EcoMart.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        public DateTime OrderDate { get; set; }

        public OrderStatus OrderStatus { get; set; }

        public int TotalAmount { get; set; }

        public PaymentStatus PaymentStatus { get; set; }

        public string ShippingAddress { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
