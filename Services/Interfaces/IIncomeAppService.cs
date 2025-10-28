using BudgetTracker.Models;

namespace BudgetTracker.Services.Interfaces
{
    public interface IIncomeAppService
    {
        Task<IEnumerable<Income>> GetAllAsync();
        Task<Income> GetByIdAsync(int id);
        Task AddAsync(Income income);
        Task UpdateAsync(Income income);
        Task DeleteAsync(int id);
    }
}
