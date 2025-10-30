using BudgetTracker.Data;
using BudgetTracker.Models;
using BudgetTracker.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BudgetTracker.Repositories.Implementations
{
    public class InvestmentRepository : GenericRepository<Investment>, IInvestmentRepository
    {
        public InvestmentRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<Investment>> GetAllByUserAsync(string userId)
        {
            return await _dbSet
                .Where(i => i.UserId == userId)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalInvestmentAsync(string userId, DateTime startDate, DateTime endDate)
        {
            return await _dbSet.Where(i => i.UserId == userId && i.DateInvested >= startDate && i.DateInvested <= endDate)
                .SumAsync(i => i.Amount);
        }

        public override async Task AddAsync(Investment investment)
        {
            _dbSet.Add(investment);
            await _context.SaveChangesAsync();
        }

        public override async Task UpdateAsync(Investment investment)
        {
            _dbSet.Update(investment);
            await _context.SaveChangesAsync();
        }

        public override async Task DeleteAsync(Investment investment)
        {
            _dbSet.Remove(investment);
            await _context.SaveChangesAsync();
        }
    }
}
