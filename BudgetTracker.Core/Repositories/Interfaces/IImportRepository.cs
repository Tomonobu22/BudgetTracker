using BudgetTracker.Core.Models;

namespace BudgetTracker.Core.Repositories.Interfaces
{
    public interface IImportRepository: IGenericRepository<Import>
    {
        Task<IEnumerable<Import>> GetAllByUserAsync(string userId);
        Task<int> CountImportsByUserAndDateAsync(string userId, DateTime date, CancellationToken cancellationToken);
    }
}
