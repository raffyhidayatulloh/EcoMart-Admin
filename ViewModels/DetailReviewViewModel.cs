using EcoMart.Models;

namespace EcoMart.ViewModels
{
    public class DetailReviewViewModel
    {
        public IQueryable<Review> Reviews { get; set; }
        public int ProductId { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalReviews { get; set; }
        public double AverageRating { get; set; }
        public int RatingCount1 { get; set; }
        public int RatingCount2 { get; set; }
        public int RatingCount3 { get; set; }
        public int RatingCount4 { get; set; }
        public int RatingCount5 { get; set; }
    }
}
