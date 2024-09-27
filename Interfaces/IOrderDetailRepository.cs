using EcoMart.Models;

namespace EcoMart.Interfaces
{
    public interface IOrderDetailRepository
    {
        Task<IEnumerable<OrderDetail>> GetAll();
        Task<OrderDetail> GetByIdAsync(int id);
        Task<OrderDetail> GetByIdAsyncNoTracking(int id);
        Task<OrderDetail> GetByOrderIdAsync(int id);
        bool Add(OrderDetail orderDetail);
        bool Update(OrderDetail orderDetail);
        bool Delete(OrderDetail orderDetail);
        bool Save();
    }
}
