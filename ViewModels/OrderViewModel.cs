using EcoMart.Data.Enum;
using EcoMart.Models;

namespace EcoMart.ViewModels
{
    public class OrderViewModel
    {
        public IQueryable<Order> Orders { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public string Term { get; set; }
        public string FilterBy { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
