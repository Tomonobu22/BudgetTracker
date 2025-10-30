using BudgetTracker.Models;

namespace BudgetTracker.Repositories.Interfaces
{
    public interface IIncomeRepository : IGenericRepository<Income>
    {
        Task<IEnumerable<Income>> GetAllByUserAsync(string userId);
        Task<decimal> GetTotalIncomeAsync(string userId, DateTime startDate, DateTime endDate);
    }
}
