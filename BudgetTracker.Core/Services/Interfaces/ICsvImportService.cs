using BudgetTracker.Core.DTOs;
using BudgetTracker.Core.Models;

namespace BudgetTracker.Core.Services.Interfaces
{
    public interface ICsvImportService
    {
        Task<IEnumerable<Expense>> ParseAsync(Stream csvStream, string userId, CancellationToken cancellationToken = default);
    }
}
