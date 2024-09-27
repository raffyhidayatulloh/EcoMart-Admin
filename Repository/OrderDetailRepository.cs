using CloudinaryDotNet.Actions;
using EcoMart.Data;
using EcoMart.Interfaces;
using EcoMart.Models;
using Microsoft.EntityFrameworkCore;

namespace EcoMart.Repository
{
    public class OrderDetailRepository : IOrderDetailRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderDetailRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool Add(OrderDetail orderDetail)
        {
            _context.Add(orderDetail);
            return Save();
        }

        public bool Delete(OrderDetail orderDetail)
        {
            _context.Remove(orderDetail);
            return Save();
        }

        public async Task<IEnumerable<OrderDetail>> GetAll()
        {
            return await _context.OrderDetails.ToListAsync();
        }

        public async Task<OrderDetail> GetByIdAsync(int id)
        {
            return await _context.OrderDetails.FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<OrderDetail> GetByIdAsyncNoTracking(int id)
        {
            return await _context.OrderDetails.AsNoTracking().FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<OrderDetail> GetByOrderIdAsync(int id)
        {
            return await _context.OrderDetails.FirstOrDefaultAsync(i => i.OrderId == id);
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool Update(OrderDetail orderDetail)
        {
            _context.Update(orderDetail);
            return Save();
        }
    }
}
