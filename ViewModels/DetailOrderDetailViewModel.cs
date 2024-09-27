using EcoMart.Models;

namespace EcoMart.ViewModels
{
    public class DetailOrderDetailViewModel
    {
        public IQueryable<OrderDetail> OrderDetails { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
