using EcoMart.Models;

namespace EcoMart.Interfaces
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetAll();
        Task<Order> GetByIdAsync(int id);
        Task<Order> GetByIdAsyncNoTracking(int id);
        bool Add(Order order);
        bool Update(Order order);
        bool Delete(Order order);
        bool Save();
    }
}
