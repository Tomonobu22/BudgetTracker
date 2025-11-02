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
                .Include(e => e.Tag)
                .OrderByDescending(e => e.DateIncurred)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalExpenseAsync(string userId, DateTime startDate, DateTime endDate)
        {
            return await _dbSet.Where(e => e.UserId == userId && e.DateIncurred >= startDate && e.DateIncurred <= endDate).SumAsync(e => e.Amount);
        }

        public async Task<List<decimal>> GetMonthlyExpenseAsync(string userId, int year)
        {
            var monthlyExpense = new List<decimal>(new decimal[12]);
            var expenses = await _dbSet
                .Where(e => e.UserId == userId && e.DateIncurred.Year == year)
                .ToListAsync();
            foreach (var expense in expenses)
            {
                int monthIndex = expense.DateIncurred.Month - 1;
                monthlyExpense[monthIndex] += expense.Amount;
            }
            return monthlyExpense;
        }

        public override async Task<Expense?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(e => e.Tag)
                .FirstOrDefaultAsync(e => e.Id == id);
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
