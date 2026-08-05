using BudgetTracker.Core.DTOs;

namespace BudgetTracker.Core.Services.Interfaces
{
    public interface IInvestmentAppService
    {
        Task<PagedResultDto<InvestmentDto>> GetPagedByUserAsync(string userId, PagingRequestDto request, InvestmentFilterDto? filter = null);
        Task<IEnumerable<InvestmentDto>> GetAllByUserAsync(string userId);
        Task<List<InvestmentDto>> GetInvestmentsByImportIdAsync(int importId, string userId);
        Task<InvestmentDto> GetByIdAsync(int id, string userId);
        Task CreateAsync(InvestmentDto dto, string userId);
        Task UpdateAsync(InvestmentDto dto, string userId);
        Task DeleteAsync(int id, string userId);
        Task<bool> HasInvestmentsWithTagAsync(int id);
    }
}
