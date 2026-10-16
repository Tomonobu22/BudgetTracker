using BudgetTracker.Core.Models;

namespace BudgetTracker.Core.Repositories.Interfaces
{
    public interface IInvestmentRepository : IGenericRepository<Investment>
    {
        IQueryable<Investment> Query(string userId);
        Task<IEnumerable<Investment>> GetAllByUserAsync(string userId);
        Task<List<decimal>> GetMonthlyInvestmentAsync(string userId, int year);
        // Returns monthly sums of the current value of investments (aligned by DateInvested month)
        Task<List<decimal>> GetMonthlyCurrentValueAsync(string userId, int year);
        // Single call to retrieve both invested amounts and current values per month to avoid multiple queries
        Task<(List<decimal> Invested, List<decimal> Current)> GetMonthlyInvestmentSummaryAsync(string userId, int year);
        Task<decimal> GetTotalInvestmentAsync(string userId, DateTime startDate, DateTime endDate);
        Task<List<int>> GetYearsWithDataAsync(string userId);
        Task AddRangeAsync(IEnumerable<Investment> investments);
    }
}
