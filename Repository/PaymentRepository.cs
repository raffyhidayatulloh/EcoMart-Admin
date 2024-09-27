using CloudinaryDotNet.Actions;
using EcoMart.Data;
using EcoMart.Interfaces;
using EcoMart.Models;
using Microsoft.EntityFrameworkCore;

namespace EcoMart.Repository
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly ApplicationDbContext _context;

        public PaymentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool Add(Payment payment)
        {
            _context.Add(payment);
            return Save();
        }

        public bool Delete(Payment payment)
        {
            _context.Remove(payment);
            return Save();
        }

        public async Task<IEnumerable<Payment>> GetAll()
        {
            return await _context.Payments.ToListAsync();
        }

        public async Task<Payment> GetByIdAsync(int id)
        {
            return await _context.Payments.FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<Payment> GetByIdAsyncNoTracking(int id)
        {
            return await _context.Payments.AsNoTracking().FirstOrDefaultAsync(i => i.Id == id);
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool Update(Payment payment)
        {
            _context.Update(payment);
            return Save();
        }
    }
}
