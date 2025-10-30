using BudgetTracker.Models;
using BudgetTracker.Repositories.Interfaces;
using BudgetTracker.Services.Interfaces;

namespace BudgetTracker.Services.Implementations
{
    public class InvestmentAppService : IInvestmentAppService
    {
        private IGenericRepository<Investment> _investmentRepository;
        public InvestmentAppService(IGenericRepository<Investment> investmentRepository)
        {
            _investmentRepository = investmentRepository;
        }
        public async Task<IEnumerable<Investment>> GetAllAsync()
        {
            return await _investmentRepository.GetAllAsync();
        }
        public async Task<Investment> GetByIdAsync(int id)
        {
            return await _investmentRepository.GetByIdAsync(id);
        }
        public async Task AddAsync(Investment investment)
        {
            await _investmentRepository.AddAsync(investment);
        }
        public async Task UpdateAsync(Investment investment)
        {
            await _investmentRepository.UpdateAsync(investment);
        }
        public async Task DeleteAsync(int id)
        {
            var investment = await _investmentRepository.GetByIdAsync(id);
            if (investment != null)
            {
                await _investmentRepository.DeleteAsync(investment);
            }
        }
    }
}
