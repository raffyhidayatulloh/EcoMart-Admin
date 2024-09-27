using EcoMart.Models;

namespace EcoMart.Interfaces
{
    public interface IReviewRepository
    {
        Task<IEnumerable<Review>> GetAll();
        Task<Review> GetByIdAsync(int id);
        Task<Review> GetByIdAsyncNoTracking(int id);
        bool Add(Review review);
        bool Update(Review review);
        bool Delete(Review review);
        bool Save();
    }
}
