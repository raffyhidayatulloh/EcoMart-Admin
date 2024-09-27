using EcoMart.Models;

namespace EcoMart.Interfaces
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAll();
        Task<Product> GetByIdAsync(int id);
        Task<int> GetByCategoryAsync(int id);
        Task<Product> GetByIdAsyncNoTracking(int id);
        bool Add(Product product);
        bool Update(Product product);
        bool Delete(Product product);
        bool Save();
    }
}
