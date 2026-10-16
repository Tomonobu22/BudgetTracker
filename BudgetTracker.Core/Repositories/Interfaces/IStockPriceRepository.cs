using BudgetTracker.Core.Models;

namespace BudgetTracker.Core.Repositories.Interfaces
{
    public interface IStockPriceRepository : IGenericRepository<StockPrice>
    {
        Task<StockPrice?> GetByTickerAsync(string ticker);
        Task<IEnumerable<StockPrice>> GetAllOrderedAsync();
    }
}
