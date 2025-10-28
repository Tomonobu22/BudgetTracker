using BudgetTracker.Models;
using BudgetTracker.Repositories.Interfaces;
using BudgetTracker.Services.Interfaces;

namespace BudgetTracker.Services.Implementations
{
    public class IncomeAppService : IIncomeAppService
    {
        private IGenericRepository<Income> _incomeRepository;
        public IncomeAppService(IGenericRepository<Income> incomeRepository)
        {
            _incomeRepository = incomeRepository;
        }
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
            await _incomeRepository.SaveChangesAsync();
        }
        public async Task UpdateAsync(Income income)
        {
            _incomeRepository.Update(income);
            await _incomeRepository.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var income = await _incomeRepository.GetByIdAsync(id);
            if (income != null)
            {
                _incomeRepository.Delete(income);
                await _incomeRepository.SaveChangesAsync();
            }
        }
    }
}
