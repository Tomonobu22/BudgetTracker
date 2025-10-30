using BudgetTracker.Data;
using BudgetTracker.Models;
using BudgetTracker.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BudgetTracker.Repositories.Implementations
{
    public class IncomeRepository : GenericRepository<Income>, IIncomeRepository
    {
        public IncomeRepository(AppDbContext context) : base(context) {}

        public async Task<IEnumerable<Income>> GetAllByUserAsync(string userId)
        {
            return await _dbSet
                .Where(i => i.UserId == userId)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalIncomeAsync(string userId, DateTime startDate, DateTime endDate)
        {
            return await _dbSet.Where(i => i.UserId == userId && i.DateReceived >= startDate && i.DateReceived <= endDate)
                .SumAsync(i => i.Amount);
        }

        public override async Task AddAsync(Income income)
        {
            _dbSet.Add(income);
            await _context.SaveChangesAsync();
        }

        public override async Task UpdateAsync(Income income)
        {
            _dbSet.Update(income);
            await _context.SaveChangesAsync();
        }

        public override async Task DeleteAsync(Income income)
        {
            _dbSet.Remove(income);
            await _context.SaveChangesAsync();
        }
    }
}
