using BudgetTracker.Models;

namespace BudgetTracker.Services.Interfaces
{
    public interface IInvestmentAppService
    {
        Task<IEnumerable<Investment>> GetAllAsync();
        Task<Investment> GetByIdAsync(int id);
        Task AddAsync(Investment investment);
        Task UpdateAsync(Investment investment);
        Task DeleteAsync(int id);
    }
}
