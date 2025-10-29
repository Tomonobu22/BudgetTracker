using BudgetTracker.Data;
using BudgetTracker.Models;
using BudgetTracker.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BudgetTracker.Repositories.Implementations
{
    public class ExpenseRepository : GenericRepository<Expense>, IExpenseRepository
    {
        public ExpenseRepository(AppDbContext context) : base(context) { }

        public async Task<decimal> GetTotalExpenseAsync(int userId, DateTime startDate, DateTime endDate)
        {
            return await _dbSet.Where(e => e.UserId == userId && e.DateIncurred >= startDate && e.DateIncurred <= endDate).SumAsync(e => e.Amount);
        }
    }
}
