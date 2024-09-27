using EcoMart.Models;

namespace EcoMart.Interfaces
{
    public interface IReportRepository
    {
        Task<IEnumerable<Report>> GetAll();
        Task<Report> GetByIdAsync(int id);
        Task<Report> GetByIdAsyncNoTracking(int id);
        bool Add(Report report);
        bool Update(Report report);
        bool Delete(Report report);
        bool Save();
    }
}
