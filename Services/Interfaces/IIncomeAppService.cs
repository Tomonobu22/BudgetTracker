using BudgetTracker.DTOs;

namespace BudgetTracker.Services.Interfaces
{
    public interface IIncomeAppService
    {
        Task<IEnumerable<IncomeDto>> GetAllByUserAsync(string userId);
        Task CreateAsync(IncomeDto dto, string userId);
        Task<IncomeDto> GetByIdAsync(int id, string userId);
        Task UpdateAsync(IncomeDto dto, string userId);
        Task DeleteAsync(int id, string userId);
        Task AddAsync(IncomeDto dto, string userId);
    }
}
