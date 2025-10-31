using BudgetTracker.Models;

namespace BudgetTracker.Repositories.Interfaces
{
    public interface IInvestmentRepository : IGenericRepository<Investment>
    {
        Task<IEnumerable<Investment>> GetAllByUserAsync(string userId);
        Task<decimal> GetTotalInvestmentAsync(string userId, DateTime startDate, DateTime endDate);
    }
}
