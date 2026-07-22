using BudgetTracker.Core.DTOs;
using BudgetTracker.Core.Enums;
using BudgetTracker.Core.Models;

namespace BudgetTracker.Core.Services.Interfaces
{
    public interface IImportAppService
    {
        Task<IEnumerable<ImportDto>> GetAllByUserAsync(string userId);
        Task<ImportDto> GetByIdAsync(int id, string userId);
        Task<ImportDto> CreateImportAsync(FileUploadRequest request, RecordType importType, string userId, CancellationToken cancellationToken);
        Task CreateAsync(ImportDto import, string userId);
        Task UpdateAsync(ImportDto import, string userId);
        Task DeleteAsync(int id, string userId);
    }
}