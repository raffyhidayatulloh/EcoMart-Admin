using EcoMart.Models;

namespace EcoMart.ViewModels
{
    public class OrderDetailViewModel
    {
        public List<OrderDetail> Details { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
