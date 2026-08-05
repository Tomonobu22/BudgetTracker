using AutoMapper;
using BudgetTracker.Core.DTOs;
using BudgetTracker.Core.Models;
using BudgetTracker.Core.Repositories.Interfaces;
using BudgetTracker.Core.Services.Interfaces;
using BudgetTracker.Core.Helpers;
using Microsoft.EntityFrameworkCore;

namespace BudgetTracker.Core.Services.Implementations
{
    public class InvestmentAppService : IInvestmentAppService
    {
        private readonly IInvestmentRepository _investmentRepository;
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;

        public InvestmentAppService(IInvestmentRepository investmentRepository, IMapper mapper, ICacheService cacheService)
        {
            _investmentRepository = investmentRepository;
            _mapper = mapper;
            _cacheService = cacheService;
        }

        public async Task<IEnumerable<InvestmentDto>> GetAllByUserAsync(string userId)
        {
            var investments = await _investmentRepository.GetAllByUserAsync(userId);
            return _mapper.Map<IEnumerable<InvestmentDto>>(investments);
        }
        public async Task CreateAsync(InvestmentDto dto, string userId)
        {
            var investment = _mapper.Map<Investment>(dto);
            investment.UserId = userId;
            await _investmentRepository.AddAsync(investment);
            _cacheService.Remove(CacheKeys.GetMonthlySummaryKey(userId, investment.DateInvested.Year));
            _cacheService.Remove(CacheKeys.AvailableYearsKey(userId));
        }
        public async Task<InvestmentDto> GetByIdAsync(int id, string userId)
        {
            var investment = await _investmentRepository.GetByIdAsync(id);
            if (investment != null && investment.UserId == userId)
            {
                return _mapper.Map<InvestmentDto>(investment);
            }
            throw new KeyNotFoundException("Investment not found or access denied.");
        }

        public async Task<bool> HasInvestmentsWithTagAsync(int tagId)
        {
            var allInvestments = await _investmentRepository.GetAllAsync();
            return allInvestments.Any(i => i.TagId == tagId);
        }

        public async Task UpdateAsync(InvestmentDto dto, string userId)
        {
            // Get the previous investment to check if the year has changed
            var investment = await _investmentRepository.GetByIdAsync(dto.Id);
            if (investment == null || investment.UserId != userId)
            {
                throw new KeyNotFoundException("Investment not found or access denied.");
            }

            var oldYear = investment.DateInvested.Year;
            _mapper.Map(dto, investment);

            await _investmentRepository.UpdateAsync(investment);
            _cacheService.Remove(CacheKeys.GetMonthlySummaryKey(userId, investment.DateInvested.Year));
            if (oldYear != investment.DateInvested.Year)
            {
                _cacheService.Remove(CacheKeys.GetMonthlySummaryKey(userId, oldYear));
                _cacheService.Remove(CacheKeys.AvailableYearsKey(userId));
            }
        }
        public async Task DeleteAsync(int id, string userId)
        {
            var investment = await _investmentRepository.GetByIdAsync(id);
            if (investment != null && investment.UserId == userId)
            {
                await _investmentRepository.DeleteAsync(investment);
                _cacheService.Remove(CacheKeys.GetMonthlySummaryKey(userId, investment.DateInvested.Year));
                _cacheService.Remove(CacheKeys.AvailableYearsKey(userId));
            }
        }

        public async Task<List<InvestmentDto>> GetInvestmentsByImportIdAsync(int importId, string userId)
        {
            var investments = await _investmentRepository.GetAllFromImportIdAsync(importId, userId);
            return _mapper.Map<List<InvestmentDto>>(investments);
        }

        public async Task<PagedResultDto<InvestmentDto>> GetPagedByUserAsync(string userId, PagingRequestDto request, InvestmentFilterDto? filter = null)
        {
            var investment = _investmentRepository.Query(userId);

            if (filter != null)
            { 
                if (!string.IsNullOrEmpty(filter.Type))
                {
                    investment = investment.Where(i => i.Tag != null && i.Tag.Name.ToLower() == filter.Type.ToLower());
                }
                if (filter.StartDate.HasValue)
                {
                    investment = investment.Where(i => i.DateInvested >= filter.StartDate.Value);
                }
                if (filter.EndDate.HasValue)
                {
                    investment = investment.Where(i => i.DateInvested <= filter.EndDate.Value);
                }
            }

            var totalCount = await investment.CountAsync();
            var totalAmount = await investment.SumAsync(i => i.Amount);

            var items = await investment
                .OrderByDescending(i => i.DateInvested)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            return new PagedResultDto<InvestmentDto>
            {
                Items = _mapper.Map<List<InvestmentDto>>(items),
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalAmount = totalAmount
            };

        }
    }
}
