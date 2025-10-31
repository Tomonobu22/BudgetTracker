using BudgetTracker.Models;

namespace BudgetTracker.Repositories.Interfaces
{
    public interface IExpenseRepository : IGenericRepository<Expense>
    {
        Task<decimal> GetTotalExpenseAsync(string userId, DateTime startDate, DateTime endDate);
        Task<List<decimal>> GetMonthlyExpenseAsync(string userId, int year);
        Task<IEnumerable<Expense>> GetAllByUserAsync(string userId);
    }
}
