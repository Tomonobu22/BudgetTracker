using BudgetTracker.DTOs;
using BudgetTracker.Models;

namespace BudgetTracker.Services.Interfaces
{
    public interface IIncomeAppService
    {
        Task<IEnumerable<IncomeDto>> GetAllByUserAsync(string userId);
        Task CreateAsync(IncomeDto dto, string userId);
        Task<Income> GetByIdAsync(int id, string userId);
        Task UpdateAsync(Income income);
        Task DeleteAsync(int id);

        // Review
        Task<IEnumerable<Income>> GetAllAsync();
        Task AddAsync(Income income);
    }
}
