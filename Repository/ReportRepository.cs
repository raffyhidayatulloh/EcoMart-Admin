using EcoMart.Data;
using EcoMart.Interfaces;
using EcoMart.Models;
using Microsoft.EntityFrameworkCore;

namespace EcoMart.Repository
{
    public class ReportRepository : IReportRepository
    {
        private readonly ApplicationDbContext _context;

        public ReportRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool Add(Report report)
        {
            _context.Add(report);
            return Save();
        }

        public bool Delete(Report report)
        {
            _context.Remove(report);
            return Save();
        }

        public async Task<IEnumerable<Report>> GetAll()
        {
            return await _context.Reports.ToListAsync();
        }

        public async Task<Report> GetByIdAsync(int id)
        {
            return await _context.Reports.Include(i => i.Product).FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<Report> GetByIdAsyncNoTracking(int id)
        {
            return await _context.Reports.AsNoTracking().FirstOrDefaultAsync(i => i.Id == id);
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool Update(Report report)
        {
            _context.Update(report);
            return Save();
        }
    }
}
