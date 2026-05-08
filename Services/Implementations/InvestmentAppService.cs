using AutoMapper;
using BudgetTracker.DTOs;
using BudgetTracker.Models;
using BudgetTracker.Repositories.Interfaces;
using BudgetTracker.Services.Interfaces;

namespace BudgetTracker.Services.Implementations
{
    public class InvestmentAppService : IInvestmentAppService
    {
        private readonly IInvestmentRepository _investmentRepository;
        private readonly IMapper _mapper;

        public InvestmentAppService(IInvestmentRepository investmentRepository, IMapper mapper)
        {
            _investmentRepository = investmentRepository;
            _mapper = mapper;
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

        public async Task UpdateAsync(InvestmentDto investment, string userId)
        {
            var entity = _mapper.Map<Investment>(investment);
            entity.UserId = userId;
            await _investmentRepository.UpdateAsync(entity);
        }
        public async Task DeleteAsync(int id, string userId)
        {
            var investment = await _investmentRepository.GetByIdAsync(id);
            if (investment != null && investment.UserId == userId)
            {
                await _investmentRepository.DeleteAsync(investment);
            }
        }
    }
}
