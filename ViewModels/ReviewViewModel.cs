namespace EcoMart.ViewModels
{
    public class ReviewViewModel
    {
        public IQueryable<ProductReviewViewModel> Reviews { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}