using AutoMapper;
using BudgetTracker.Core.DTOs;
using BudgetTracker.Core.Models;
using BudgetTracker.Core.Repositories.Interfaces;
using BudgetTracker.Core.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace BudgetTracker.Core.Services.Implementations
{
    public class StockPriceAppService : IStockPriceAppService
    {
        private readonly IStockPriceRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<TagAppService> _logger;
        private readonly ICacheService _cacheService;

        public StockPriceAppService(IStockPriceRepository repository, 
                                    IMapper mapper,
                                    ILogger<TagAppService> logger,
                                    ICacheService cacheService)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
            _cacheService = cacheService;
        }

        public async Task<IEnumerable<StockPriceDto>> GetAllAsync()
        {
            var cacheKey = "StockPrices_All";
            var cachedItems = _cacheService.Get<IEnumerable<StockPriceDto>>(cacheKey);
            if (cachedItems != null)
            {
                _logger.LogDebug($"Cache hit {cacheKey}");
                return cachedItems;
            }

            var items = await _repository.GetAllOrderedAsync();
            var result = _mapper.Map<IEnumerable<StockPriceDto>>(items);
            _cacheService.Set(cacheKey, result, TimeSpan.FromMinutes(10));
            _logger.LogDebug($"Cache set {cacheKey}");
            return result;
        }

        public async Task<StockPriceDto?> GetByTickerAsync(string ticker)
        {
            var item = await _repository.GetByTickerAsync(ticker);
            return item == null ? null : _mapper.Map<StockPriceDto>(item);
        }

        public async Task UpsertAsync(StockPriceDto dto)
        {
            var existing = await _repository.GetByTickerAsync(dto.Ticker);
            if (existing == null)
            {
                var entity = _mapper.Map<StockPrice>(dto);
                await _repository.AddAsync(entity);
            }
            else
            {
                _mapper.Map(dto, existing);
                await _repository.UpdateAsync(existing);
            }

            // Invalidate cache
            _cacheService.Remove("StockPrices_All");
        }

        public async Task DeleteAsync(string ticker)
        {
            var existing = await _repository.GetByTickerAsync(ticker);
            if (existing != null)
            {
                await _repository.DeleteAsync(existing);
                // Invalidate cache
                _cacheService.Remove("StockPrices_All");
            }
        }
    }
}
