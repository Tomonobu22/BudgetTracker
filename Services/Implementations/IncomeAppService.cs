using AutoMapper;
using BudgetTracker.DTOs;
using BudgetTracker.Models;
using BudgetTracker.Repositories.Interfaces;
using BudgetTracker.Services.Interfaces;

namespace BudgetTracker.Services.Implementations
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
            var incomes = await _incomeRepository.GetAllByUserAsync(userId);
            return _mapper.Map<IEnumerable<IncomeDto>>(incomes);
        }

        public async Task CreateAsync(IncomeDto dto, string userId)
        {
            var income = _mapper.Map<Income>(dto);
            income.UserId = userId;
            await _incomeRepository.AddAsync(income);
        }


        // Review

        public async Task<IEnumerable<Income>> GetAllAsync()
        {
            return await _incomeRepository.GetAllAsync();
        }
        public async Task<Income> GetByIdAsync(int id)
        {
            return await _incomeRepository.GetByIdAsync(id);
        }
        public async Task AddAsync(Income income)
        {
            await _incomeRepository.AddAsync(income);
        }
        public async Task UpdateAsync(Income income)
        {
            await _incomeRepository.UpdateAsync(income);
        }
        public async Task DeleteAsync(int id)
        {
            var income = await _incomeRepository.GetByIdAsync(id);
            if (income != null)
            {
                await _incomeRepository.DeleteAsync(income);
            }
        }

        public Task<Income> GetByIdAsync(int id, string userId)
        {
            throw new NotImplementedException();
        }
    }
}
