using BudgetTracker.Data;
using BudgetTracker.Models;
using BudgetTracker.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BudgetTracker.Repositories.Implementations
{
    public class InvestmentRepository : GenericRepository<Investment>, IInvestmentRepository
    {
        public InvestmentRepository(AppDbContext context) : base(context) { }

        public async Task<decimal> GetTotalInvestmentAsync(string userId, DateTime startDate, DateTime endDate)
        {
            return await _dbSet.Where(i => i.UserId == userId && i.DateInvested >= startDate && i.DateInvested <= endDate)
                .SumAsync(i => i.Amount);
        }
    }
}
