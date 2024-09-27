using EcoMart.Models;

namespace EcoMart.ViewModels
{
    public class CouponViewModel
    {
        public IQueryable<Coupon> Coupons { get; set; }
        public string FilterBy { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public CreateCouponViewModel CreateCouponVM { get; set; }
    }
}
