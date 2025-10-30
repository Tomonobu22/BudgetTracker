using BudgetTracker.Data;
using BudgetTracker.Models;
using BudgetTracker.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BudgetTracker.Repositories.Implementations
{
    public class ExpenseRepository : GenericRepository<Expense>, IExpenseRepository
    {
        public ExpenseRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<Expense>> GetAllByUserAsync(string userId)
        {
            return await _dbSet
                .Where(e => e.UserId == userId)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalExpenseAsync(string userId, DateTime startDate, DateTime endDate)
        {
            return await _dbSet.Where(e => e.UserId == userId && e.DateIncurred >= startDate && e.DateIncurred <= endDate).SumAsync(e => e.Amount);
        }

        public override async Task AddAsync(Expense expense)
        {
            _dbSet.Add(expense);
            await _context.SaveChangesAsync();
        }
        public override async Task UpdateAsync(Expense expense)
        {
            _dbSet.Update(expense);
            await _context.SaveChangesAsync();
        }
        public override async Task DeleteAsync(Expense expense)
        {
            _dbSet.Remove(expense);
            await _context.SaveChangesAsync();
        }
    }
}
