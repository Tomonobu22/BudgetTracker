using BudgetTracker.Core.Models;

namespace BudgetTracker.Core.Repositories.Interfaces
{
    public interface IIncomeRepository : IGenericRepository<Income>
    {
        Task<IEnumerable<Income>> GetAllByUserAsync(string userId);
        Task<decimal> GetTotalIncomeAsync(string userId, DateTime startDate, DateTime endDate);
        Task<List<decimal>> GetMonthlyIncomeAsync(string userId, int year);
        Task<List<int>> GetYearsWithDataAsync(string userId);
    }
}
