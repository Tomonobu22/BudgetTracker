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
                .Include(i => i.Tag)
                .OrderByDescending(i => i.DateReceived)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalIncomeAsync(string userId, DateTime startDate, DateTime endDate)
        {
            return await _dbSet.Where(i => i.UserId == userId && i.DateReceived >= startDate && i.DateReceived <= endDate)
                .SumAsync(i => i.Amount);
        }

        public async Task<List<decimal>> GetMonthlyIncomeAsync(string userId, int year)
        {
            var monthlyIncome = new List<decimal>(new decimal[12]);
            var incomes = await _dbSet
                .Where(i => i.UserId == userId && i.DateReceived.Year == year)
                .ToListAsync();
            foreach (var income in incomes)
            {
                int monthIndex = income.DateReceived.Month - 1;
                monthlyIncome[monthIndex] += income.Amount;
            }
            return monthlyIncome;
        }

        public override async Task<Income?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(i => i.Tag)
                .FirstOrDefaultAsync(i => i.Id == id);
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
