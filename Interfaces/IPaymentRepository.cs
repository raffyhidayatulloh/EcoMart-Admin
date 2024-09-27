using EcoMart.Models;

namespace EcoMart.Interfaces
{
    public interface IPaymentRepository
    {
        Task<IEnumerable<Payment>> GetAll();
        Task<Payment> GetByIdAsync(int id);
        Task<Payment> GetByIdAsyncNoTracking(int id);
        bool Add(Payment payment);
        bool Update(Payment payment);
        bool Delete(Payment payment);
        bool Save();
    }
}
