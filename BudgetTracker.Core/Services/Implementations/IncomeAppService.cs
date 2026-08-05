using AutoMapper;
using BudgetTracker.Core.DTOs;
using BudgetTracker.Core.Models;
using BudgetTracker.Core.Repositories.Interfaces;
using BudgetTracker.Core.Services.Interfaces;
using BudgetTracker.Core.Helpers;
using Microsoft.EntityFrameworkCore;

namespace BudgetTracker.Core.Services.Implementations
{
    public class IncomeAppService : IIncomeAppService
    {
        private readonly IIncomeRepository _incomeRepository;
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;

        public IncomeAppService(IIncomeRepository incomeRepository, IMapper mapper, ICacheService cacheService)
        {
            _incomeRepository = incomeRepository;
            _mapper = mapper;
            _cacheService = cacheService;
        }

        public async Task<IEnumerable<IncomeDto>> GetAllByUserAsync(string userId)
        {
            var income = await _incomeRepository.GetAllByUserAsync(userId);
            return _mapper.Map<IEnumerable<IncomeDto>>(income);
        }

        public async Task<bool> HasIncomesWithTagAsync(int tagId)
        {
            var allIncomes = await _incomeRepository.GetAllAsync();
            return allIncomes.Any(i => i.TagId == tagId);
        }

        public async Task CreateAsync(IncomeDto dto, string userId)
        {
            var income = _mapper.Map<Income>(dto);
            income.UserId = userId;
            await _incomeRepository.AddAsync(income);
            _cacheService.Remove(CacheKeys.GetMonthlySummaryKey(userId, income.DateReceived.Year));
            _cacheService.Remove(CacheKeys.AvailableYearsKey(userId));
        }

        public async Task AddAsync(IncomeDto dto, string userId)
        {
            var income = _mapper.Map<Income>(dto);
            income.UserId = userId;
            await _incomeRepository.AddAsync(income);
            _cacheService.Remove(CacheKeys.GetMonthlySummaryKey(userId, income.DateReceived.Year));
            _cacheService.Remove(CacheKeys.AvailableYearsKey(userId));
        }
        public async Task UpdateAsync(IncomeDto dto, string userId)
        {
            // Get the previous income record to check if the year has changed
            var income = await _incomeRepository.GetByIdAsync(dto.Id);
            if (income == null || income.UserId != userId)
            {
                throw new KeyNotFoundException("Income not found or access denied.");
            }

            var oldYear = income.DateReceived.Year;
            _mapper.Map(dto, income);

            await _incomeRepository.UpdateAsync(income);
            _cacheService.Remove(CacheKeys.GetMonthlySummaryKey(userId, income.DateReceived.Year));
            if (oldYear != dto.DateReceived.Year)
            {
                _cacheService.Remove(CacheKeys.GetMonthlySummaryKey(userId, oldYear));
                _cacheService.Remove(CacheKeys.AvailableYearsKey(userId));
            }
        }
        public async Task DeleteAsync(int id, string userId)
        {
            var income = await _incomeRepository.GetByIdAsync(id);
            if (income != null && income.UserId == userId)
            {
                await _incomeRepository.DeleteAsync(income);
                _cacheService.Remove(CacheKeys.GetMonthlySummaryKey(userId, income.DateReceived.Year));
                _cacheService.Remove(CacheKeys.AvailableYearsKey(userId));
            }
        }

        public async Task<IncomeDto> GetByIdAsync(int id, string userId)
        {
            var income = await _incomeRepository.GetByIdAsync(id);
            if (income != null && income.UserId == userId)
            {
                return _mapper.Map<IncomeDto>(income);
            }
            throw new KeyNotFoundException("Expense not found or access denied.");
        }

        public async Task<List<IncomeDto>> GetIncomesByImportIdAsync(int importId, string userId)
        {
            var incomes = await _incomeRepository.GetAllFromImportIdAsync(importId, userId);
            return _mapper.Map<List<IncomeDto>>(incomes);
        }

        public async Task<PagedResultDto<IncomeDto>> GetPagedByUserAsync(string userId, PagingRequestDto request, IncomeFilterDto? filter = null)
        {

            var income = _incomeRepository.Query(userId);
            
            if (filter != null)
            {
                if (!string.IsNullOrEmpty(filter.Source))
                {
                    income = income.Where(i => i.Tag != null && i.Tag.Name.ToLower() == filter.Source.ToLower());
                }
                if (!string.IsNullOrEmpty(filter.Description))
                {
                    income = income.Where(i => i.Description != null && i.Description.Contains(filter.Description));
                }
                if (filter.StartDate.HasValue)
                {
                    income = income.Where(i => i.DateReceived >= filter.StartDate.Value);
                }
                if (filter.EndDate.HasValue)
                {
                    income = income.Where(i => i.DateReceived <= filter.EndDate.Value);
                }
            }

            var totalCount = await income.CountAsync();
            var totalAmount = await income.SumAsync(i => i.Amount);

            var items = await income
                .OrderByDescending(i => i.DateReceived)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();


            return new PagedResultDto<IncomeDto>
            {
                Items = _mapper.Map<List<IncomeDto>>(items),
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalAmount = totalAmount
            };
        }
    }
}
