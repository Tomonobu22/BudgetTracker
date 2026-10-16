using BudgetTracker.Core.DTOs;

namespace BudgetTracker.Core.Services.Interfaces
{
    public interface IStockPriceAppService
    {
        Task<IEnumerable<StockPriceDto>> GetAllAsync();
        Task<StockPriceDto?> GetByTickerAsync(string ticker);
        Task UpsertAsync(StockPriceDto dto);
        Task DeleteAsync(string ticker);
    }
}
