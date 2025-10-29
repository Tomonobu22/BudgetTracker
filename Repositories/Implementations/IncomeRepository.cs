using BudgetTracker.Data;
using BudgetTracker.Models;
using BudgetTracker.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BudgetTracker.Repositories.Implementations
{
    public class IncomeRepository : GenericRepository<Income>, IIncomeRepository
    {
        public IncomeRepository(AppDbContext context) : base(context) { }

        public async Task<decimal> GetTotalIncomeAsync(int userId, DateTime startDate, DateTime endDate)
        {
            return await _dbSet.Where(i => i.UserId == userId && i.DateReceived >= startDate && i.DateReceived <= endDate)
                .SumAsync(i => i.Amount);
        }
        // Implement other methods from IGenericRepository<Income> here...
    }
}
