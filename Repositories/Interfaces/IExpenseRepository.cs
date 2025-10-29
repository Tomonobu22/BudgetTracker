using BudgetTracker.Models;

namespace BudgetTracker.Repositories.Interfaces
{
    public interface IExpenseRepository : IGenericRepository<Expense>
    {
        Task<decimal> GetTotalExpenseAsync(int userId, DateTime startDate, DateTime endDate);
    }
}
