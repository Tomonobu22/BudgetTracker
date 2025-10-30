using BudgetTracker.DTOs;

namespace BudgetTracker.Services.Interfaces
{
    public interface IInvestmentAppService
    {
        Task<IEnumerable<InvestmentDto>> GetAllByUserAsync(string userId);
        Task<InvestmentDto> GetByIdAsync(int id, string userId);
        Task CreateAsync(InvestmentDto dto, string userId);
        Task UpdateAsync(InvestmentDto investment, string userId);
        Task DeleteAsync(int id, string userId);
    }
}
