using BudgetTracker.DTOs;
using BudgetTracker.Models;

namespace BudgetTracker.Services.Interfaces
{
    public interface IImportAppService
    {
        Task<IEnumerable<ImportDto>> GetAllByUserAsync(string userId);
        Task<ImportDto> GetByIdAsync(int id, string userId);
        Task<ImportDto> CreateImportAsync(IFormFile file, string userId, CancellationToken cancellationToken);
        Task CreateAsync(ImportDto import, string userId);
        Task UpdateAsync(ImportDto import, string userId);
        Task DeleteAsync(int id, string userId);
    }
}