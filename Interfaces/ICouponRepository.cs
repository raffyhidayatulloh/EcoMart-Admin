using EcoMart.Models;

namespace EcoMart.Interfaces
{
    public interface ICouponRepository
    {
        Task<IEnumerable<Coupon>> GetAll();
        Task<Coupon> GetByIdAsync(int id);
        Task<Coupon> GetByIdAsyncNoTracking(int id);
        bool Add(Coupon coupon);
        bool Update(Coupon coupon);
        bool Delete(Coupon coupon);
        bool Save();
    }
}
