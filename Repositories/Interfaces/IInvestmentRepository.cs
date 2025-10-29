using BudgetTracker.Models;

namespace BudgetTracker.Repositories.Interfaces
{
    public interface IInvestmentRepository : IGenericRepository<Investment>
    {
        Task<decimal> GetTotalInvestmentAsync(int userId, DateTime startDate, DateTime endDate);
    }
}
