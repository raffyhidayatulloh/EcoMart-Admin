using EcoMart.Data;
using EcoMart.Interfaces;
using EcoMart.Models;
using Microsoft.EntityFrameworkCore;

namespace EcoMart.Repository
{
    public class CouponRepository : ICouponRepository
    {
        private readonly ApplicationDbContext _context;

        public CouponRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool Add(Coupon coupon)
        {
            _context.Add(coupon);
            return Save();
        }

        public bool Delete(Coupon coupon)
        {
            _context.Remove(coupon);
            return Save();
        }

        public async Task<IEnumerable<Coupon>> GetAll()
        {
            return await _context.Coupons.ToListAsync();
        }

        public async Task<Coupon> GetByIdAsync(int id)
        {
            return await _context.Coupons.FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<Coupon> GetByIdAsyncNoTracking(int id)
        {
            return await _context.Coupons.AsNoTracking().FirstOrDefaultAsync(i => i.Id == id);
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool Update(Coupon coupon)
        {
            _context.Update(coupon);
            return Save();
        }
    }
}
