using AutoMapper;
using BudgetTracker.Core.DTOs;
using BudgetTracker.Core.Helpers;
using BudgetTracker.Core.Models;
using BudgetTracker.Core.Repositories.Interfaces;
using BudgetTracker.Core.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BudgetTracker.Core.Services.Implementations
{
    public class ExpenseAppService : IExpenseAppService
    {
        private readonly IExpenseRepository _expenseRepository;
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;

        public ExpenseAppService(IExpenseRepository expenseRepository, IMapper mapper, ICacheService cacheService)
        {
            _expenseRepository = expenseRepository;
            _mapper = mapper;
            _cacheService = cacheService;
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
            _cacheService.Remove(CacheKeys.GetMonthlySummaryKey(userId, expense.DateIncurred.Year));
            _cacheService.Remove(CacheKeys.AvailableYearsKey(userId));
        }
        public async Task UpdateAsync(ExpenseDto expenseDto, string userId)
        {
            // Get the previous expense record to check if the year has changed
            var expense = await _expenseRepository.GetByIdAsync(expenseDto.Id);
            if (expense == null || expense.UserId != userId)
            {
                throw new KeyNotFoundException("Expense not found or access denied.");
            }

            var oldYear = expense.DateIncurred.Year;
            _mapper.Map(expenseDto, expense);

            await _expenseRepository.UpdateAsync(expense);
            _cacheService.Remove(CacheKeys.GetMonthlySummaryKey(userId, expense.DateIncurred.Year));
            if (oldYear != expense.DateIncurred.Year)
            {
                _cacheService.Remove(CacheKeys.GetMonthlySummaryKey(userId, oldYear));
                _cacheService.Remove(CacheKeys.AvailableYearsKey(userId));
            }
        }
        public async Task DeleteAsync(int id, string userId)
        {
            var expense = await _expenseRepository.GetByIdAsync(id);
            if (expense != null && expense.UserId == userId)
            {
                await _expenseRepository.DeleteAsync(expense);
                _cacheService.Remove(CacheKeys.GetMonthlySummaryKey(userId, expense.DateIncurred.Year));
                _cacheService.Remove(CacheKeys.AvailableYearsKey(userId));
            }
        }

        public async Task<List<ExpenseDto>> GetExpensesByImportIdAsync(int importId, string userId)
        {
            var expenses = await _expenseRepository.GetAllFromImportIdAsync(importId, userId);
            return _mapper.Map<List<ExpenseDto>>(expenses);
        }

        public async Task<PagedResultDto<ExpenseDto>> GetPagedByUserAsync(string userId, PagingRequestDto request, ExpenseFilterDto? filter = null)
        {
            var expense = _expenseRepository.Query(userId);
            if (filter != null)
            {
                if (!string.IsNullOrEmpty(filter.Category))
                {
                    expense = expense.Where(e => e.Tag != null && e.Tag.Name.Contains(filter.Category));
                }
                if (!string.IsNullOrEmpty(filter.Description))
                {
                    expense = expense.Where(e => e.Description != null && e.Description.Contains(filter.Description));
                }
                if (filter.StartDate.HasValue)
                {
                    expense = expense.Where(e => e.DateIncurred >= filter.StartDate.Value);
                }
                if (filter.EndDate.HasValue)
                {
                    expense = expense.Where(e => e.DateIncurred <= filter.EndDate.Value);
                }
            }

            var totalCount = await expense.CountAsync();
            var totalAmount = await expense.SumAsync(e => e.Amount);

            var items = await expense
                .OrderByDescending(e => e.DateIncurred)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            return new PagedResultDto<ExpenseDto>
            {
                Items = _mapper.Map<List<ExpenseDto>>(items),
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalAmount = totalAmount
            };
        }
    }
}
