using AutoMapper;
using BudgetTracker.DTOs;
using BudgetTracker.Models;
using BudgetTracker.Repositories.Interfaces;
using BudgetTracker.Services.Interfaces;

namespace BudgetTracker.Services.Implementations
{
    public class ExpenseAppService : IExpenseAppService
    {
        private readonly IExpenseRepository _expenseRepository;
        private readonly IMapper _mapper;

        public ExpenseAppService(IExpenseRepository expenseRepository, IMapper mapper)
        {
            _expenseRepository = expenseRepository;
            _mapper = mapper;
        }
        public async Task<IEnumerable<ExpenseDto>> GetAllByUserAsync(string userId)
        {
            var expenses = await _expenseRepository.GetAllByUserAsync(userId);
            return _mapper.Map<IEnumerable<ExpenseDto>>(expenses);
        }

        public async Task<ExpenseDto> GetByIdAsync(int id, string userId)
        {
            var expense = await _expenseRepository.GetByIdAsync(id);
            if (expense != null && expense.UserId == userId)
            {
                return _mapper.Map<ExpenseDto>(expense);
            }
            throw new KeyNotFoundException("Expense not found or access denied.");
        }

        public async Task<bool>HasExpensesWithTagAsync(int tagId)
        { 
            var allExpenses = await _expenseRepository.GetAllAsync();
            return allExpenses.Any(e => e.TagId == tagId);

        }
        public async Task CreateAsync(ExpenseDto expenseDto, string userId)
        {
            var expense = _mapper.Map<Expense>(expenseDto);
            expense.UserId = userId;
            await _expenseRepository.AddAsync(expense);
        }
        public async Task UpdateAsync(ExpenseDto expenseDto, string userId)
        {
            var expense = _mapper.Map<Expense>(expenseDto);
            expense.UserId = userId;
            await _expenseRepository.UpdateAsync(expense);
        }
        public async Task DeleteAsync(int id, string userId)
        {
            var expense = await _expenseRepository.GetByIdAsync(id);
            if (expense != null && expense.UserId == userId)
            {
                await _expenseRepository.DeleteAsync(expense);
            }
        }
    }
}
