using BudgetTracker.Models;

namespace BudgetTracker.Repositories.Interfaces
{
    public interface IIncomeRepository : IGenericRepository<Income>
    {
        Task<decimal> GetTotalIncomeAsync(int userId, DateTime startDate, DateTime endDate);
    }
}
