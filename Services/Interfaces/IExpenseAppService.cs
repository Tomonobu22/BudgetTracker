using BudgetTracker.Models;

namespace BudgetTracker.Services.Interfaces
{
    public interface IExpenseAppService
    {
        Task<IEnumerable<Expense>> GetAllAsync();
        Task<Expense> GetByIdAsync(int id);
        Task AddAsync(Expense expense);
        Task UpdateAsync(Expense expense);
        Task DeleteAsync(int id);
    }
}
