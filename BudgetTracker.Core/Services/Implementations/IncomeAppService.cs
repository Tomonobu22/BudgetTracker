using AutoMapper;
using BudgetTracker.Core.DTOs;
using BudgetTracker.Core.Models;
using BudgetTracker.Core.Repositories.Interfaces;
using BudgetTracker.Core.Services.Interfaces;

namespace BudgetTracker.Core.Services.Implementations
{
    public class IncomeAppService : IIncomeAppService
    {
        private readonly IIncomeRepository _incomeRepository;
        private readonly IMapper _mapper;

        public IncomeAppService(IIncomeRepository incomeRepository, IMapper mapper)
        {
            _incomeRepository = incomeRepository;
            _mapper = mapper;
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
        }

        public async Task AddAsync(IncomeDto dto, string userId)
        {
            var income = _mapper.Map<Income>(dto);
            income.UserId = userId;
            await _incomeRepository.AddAsync(income);
        }
        public async Task UpdateAsync(IncomeDto dto, string userId)
        {
            var income = _mapper.Map<Income>(dto);
            income.UserId = userId;
            await _incomeRepository.UpdateAsync(income);
        }
        public async Task DeleteAsync(int id, string userId)
        {
            var income = await _incomeRepository.GetByIdAsync(id);
            if (income != null && income.UserId == userId)
            {
                await _incomeRepository.DeleteAsync(income);
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
    }
}
