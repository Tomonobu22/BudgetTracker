using BudgetTracker.Core.Data;
using BudgetTracker.Core.Models;
using BudgetTracker.Core.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BudgetTracker.Core.Repositories.Implementations
{
    public class InvestmentRepository : GenericRepository<Investment>, IInvestmentRepository
    {
        public InvestmentRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<Investment>> GetAllByUserAsync(string userId)
        {
            return await _dbSet
                .Where(i => i.UserId == userId)
                .Include(i => i.Tag)
                .Include(i => i.StockPrice)
                .OrderByDescending(i => i.DateInvested)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalInvestmentAsync(string userId, DateTime startDate, DateTime endDate)
        {
            return await _dbSet.Where(i => i.UserId == userId && i.DateInvested >= startDate && i.DateInvested <= endDate)
                .SumAsync(i => i.PurchasePrice * i.Quantity);
        }

        public async Task<List<decimal>> GetMonthlyInvestmentAsync(string userId, int year)
        {
            var monthlyInvestment = new List<decimal>(new decimal[12]);
            var investments = await _dbSet
                .Where(i => i.UserId == userId && i.DateInvested.Year == year)
                .ToListAsync();
            foreach (var investment in investments)
            {
                int monthIndex = investment.DateInvested.Month - 1;
                monthlyInvestment[monthIndex] += investment.PurchasePrice * investment.Quantity;
            }
            return monthlyInvestment;
        }

        public async Task<List<decimal>> GetMonthlyCurrentValueAsync(string userId, int year)
        {
            var monthlyCurrent = new List<decimal>(new decimal[12]);
            var investments = await _dbSet
                .Where(i => i.UserId == userId && i.DateInvested.Year == year)
                .ToListAsync();
            foreach (var investment in investments)
            {
                int monthIndex = investment.DateInvested.Month - 1;
                // Use CurrentValue property which is expected to represent the current total value for that investment
                monthlyCurrent[monthIndex] += investment.CurrentValue;
            }
            return monthlyCurrent;
        }

        public async Task<(List<decimal> Invested, List<decimal> Current)> GetMonthlyInvestmentSummaryAsync(string userId, int year)
        {
            var monthlyInvested = new List<decimal>(new decimal[12]);
            var monthlyCurrent = new List<decimal>(new decimal[12]);

            var investments = await _dbSet
                .Where(i => i.UserId == userId && i.DateInvested.Year == year)
                .ToListAsync();

            foreach (var investment in investments)
            {
                int monthIndex = investment.DateInvested.Month - 1;
                monthlyInvested[monthIndex] += investment.PurchasePrice * investment.Quantity;
                monthlyCurrent[monthIndex] += investment.CurrentValue;
            }

            return (monthlyInvested, monthlyCurrent);
        }

        public override async Task<Investment?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(i => i.Tag)
                .Include(i => i.StockPrice)
                .FirstOrDefaultAsync(i => i.Id == id);
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

        public async Task<List<int>> GetYearsWithDataAsync(string userId)
        {
            return await _dbSet
                .Where(i => i.UserId == userId)
                .Select(i => i.DateInvested.Year)
                .Distinct()
                .ToListAsync();
        }

        public Task AddRangeAsync(IEnumerable<Investment> investments)
        {
            foreach (var investment in investments)
            {
                _dbSet.Add(investment);
            }
            return _context.SaveChangesAsync();
        }

        public IQueryable<Investment> Query(string userId)
        {
            return _dbSet.Where(i => i.UserId == userId).Include(i => i.Tag).Include(i => i.StockPrice);
        }
    }
}
