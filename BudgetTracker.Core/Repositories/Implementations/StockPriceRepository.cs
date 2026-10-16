using BudgetTracker.Core.Data;
using BudgetTracker.Core.Models;
using BudgetTracker.Core.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BudgetTracker.Core.Repositories.Implementations
{
    public class StockPriceRepository : GenericRepository<StockPrice>, IStockPriceRepository
    {
        public StockPriceRepository(AppDbContext context) : base(context) { }

        public async Task<StockPrice?> GetByTickerAsync(string ticker)
        {
            return await _dbSet.FirstOrDefaultAsync(s => s.Ticker == ticker);
        }

        public async Task<IEnumerable<StockPrice>> GetAllOrderedAsync()
        {
            return await _dbSet.OrderBy(s => s.Ticker).ToListAsync();
        }

        public override async Task AddAsync(StockPrice entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public override async Task UpdateAsync(StockPrice entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }

        public override async Task DeleteAsync(StockPrice entity)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
