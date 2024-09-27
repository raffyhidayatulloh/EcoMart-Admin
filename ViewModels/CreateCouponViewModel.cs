namespace EcoMart.ViewModels
{
    public class CreateCouponViewModel
    {
        public int Id { get; set; }
        public int DiscountPersent { get; set; }
        public int MaxDiscount { get; set; }
        public int MinPurchase { get; set; }
        public bool IsActive { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}
