using CloudinaryDotNet.Actions;
using EcoMart.Data;
using EcoMart.Interfaces;
using EcoMart.Models;
using Microsoft.EntityFrameworkCore;

namespace EcoMart.Repository
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly ApplicationDbContext _context;

        public ReviewRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool Add(Review review)
        {
            _context.Add(review);
            return Save();
        }

        public bool Delete(Review review)
        {
            _context.Remove(review);
            return Save();
        }

        public async Task<IEnumerable<Review>> GetAll()
        {
            return await _context.Reviews.ToListAsync();
        }

        public async Task<Review> GetByIdAsync(int id)
        {
            return await _context.Reviews.FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<Review> GetByIdAsyncNoTracking(int id)
        {
            return await _context.Reviews.AsNoTracking().FirstOrDefaultAsync(i => i.Id == id);
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool Update(Review review)
        {
            _context.Update(review);
            return Save();
        }
    }
}
