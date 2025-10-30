using BudgetTracker.Models;
using BudgetTracker.Repositories.Interfaces;
using BudgetTracker.Services.Interfaces;

namespace BudgetTracker.Services.Implementations
{
    public class ExpenseAppService : IExpenseAppService
    {
        private IGenericRepository<Expense> _expenseRepository;
        public ExpenseAppService(IGenericRepository<Expense> expenseRepository)
        {
            _expenseRepository = expenseRepository;
        }
        public async Task<IEnumerable<Expense>> GetAllAsync()
        {
            return await _expenseRepository.GetAllAsync();
        }
        public async Task<Expense> GetByIdAsync(int id)
        {
            return await _expenseRepository.GetByIdAsync(id);
        }
        public async Task AddAsync(Expense expense)
        {
            await _expenseRepository.AddAsync(expense);
        }
        public async Task UpdateAsync(Expense expense)
        {
            await _expenseRepository.UpdateAsync(expense);
        }
        public async Task DeleteAsync(int id)
        {
            var expense = await _expenseRepository.GetByIdAsync(id);
            if (expense != null)
            {
                await _expenseRepository.DeleteAsync(expense);
            }
        }
    }
}
