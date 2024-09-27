using EcoMart.Data;
using EcoMart.Interfaces;
using EcoMart.Models;
using Microsoft.EntityFrameworkCore;

namespace EcoMart.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool Add(Order order)
        {
            _context.Add(order);
            return Save();
        }

        public bool Delete(Order order)
        {
            _context.Remove(order);
            return Save();
        }

        public async Task<IEnumerable<Order>> GetAll()
        {
            return await _context.Orders.ToListAsync();
        }

        public async Task<Order> GetByIdAsync(int id)
        {
            return await _context.Orders.FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<Order> GetByIdAsyncNoTracking(int id)
        {
            return await _context.Orders.AsNoTracking().FirstOrDefaultAsync(i => i.Id == id);
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool Update(Order order)
        {
            _context.Update(order);
            return Save();
        }
    }
}
