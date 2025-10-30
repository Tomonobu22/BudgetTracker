using BudgetTracker.Models;

namespace BudgetTracker.Repositories.Interfaces
{
    public interface IInvestmentRepository : IGenericRepository<Investment>
    {
        Task<decimal> GetTotalInvestmentAsync(string userId, DateTime startDate, DateTime endDate);
    }
}
