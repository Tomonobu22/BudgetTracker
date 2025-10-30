using BudgetTracker.Models;

namespace BudgetTracker.Repositories.Interfaces
{
    public interface IExpenseRepository : IGenericRepository<Expense>
    {
        Task<decimal> GetTotalExpenseAsync(string userId, DateTime startDate, DateTime endDate);
        Task<IEnumerable<Expense>> GetAllByUserAsync(string userId);
    }
}
