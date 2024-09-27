using EcoMart.Data;
using EcoMart.Interfaces;
using EcoMart.Models;
using Microsoft.EntityFrameworkCore;

namespace EcoMart.Repository
{
    public class ShippingRepository : IShippingRepository
    {
        private readonly ApplicationDbContext _context;

        public ShippingRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool Add(Shipping shipping)
        {
            _context.Add(shipping);
            return Save();
        }

        public bool Delete(Shipping shipping)
        {
            _context.Remove(shipping);
            return Save();
        }

        public async Task<IEnumerable<Shipping>> GetAll()
        {
            return await _context.Shippings.ToListAsync();
        }

        public async Task<Shipping> GetByIdAsync(int id)
        {
            return await _context.Shippings.FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<Shipping> GetByOrderIdAsync(int id)
        {
            return await _context.Shippings.FirstOrDefaultAsync(i => i.OrderId == id);
        }

        public async Task<Shipping> GetByIdAsyncNoTracking(int id)
        {
            return await _context.Shippings.AsNoTracking().FirstOrDefaultAsync(i => i.Id == id);
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool Update(Shipping shipping)
        {
            _context.Update(shipping);
            return Save();
        }
    }
}
