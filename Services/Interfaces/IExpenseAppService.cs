using BudgetTracker.DTOs;

namespace BudgetTracker.Services.Interfaces
{
    public interface IExpenseAppService
    {
        Task<IEnumerable<ExpenseDto>> GetAllByUserAsync(string userId);
        Task<ExpenseDto> GetByIdAsync(int id, string userId);
        Task CreateAsync(ExpenseDto expense, string userId);
        Task UpdateAsync(ExpenseDto  expense, string userId);
        Task DeleteAsync(int id, string userId);
        Task<bool>HasExpensesWithTagAsync(int tagId);
    }
}
