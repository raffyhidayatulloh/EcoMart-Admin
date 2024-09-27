using EcoMart.Models;

namespace EcoMart.ViewModels
{
    public class DashboardViewModel
    {
        public IQueryable<Order> Orders { get; set; }
        public IQueryable<BestSellerProducts> BestProducts { get; set; }
        public IQueryable<TopRatedProducts> TopProducts { get; set; }
        public int PaidSuccessfully { get; set; }
        public int NotYetPaid { get; set; }
        public int OrderDelivered { get; set; }
        public int OrderPending { get; set; }
        public int TotalOrders { get; set; }
    }
}
