using EcoMart.Models;

namespace EcoMart.Interfaces
{
    public interface IShippingRepository
    {
        Task<IEnumerable<Shipping>> GetAll();
        Task<Shipping> GetByIdAsync(int id);
        Task<Shipping> GetByOrderIdAsync(int id);
        Task<Shipping> GetByIdAsyncNoTracking(int id);
        bool Add(Shipping shipping);
        bool Update(Shipping shipping);
        bool Delete(Shipping shipping);
        bool Save();
    }
}
