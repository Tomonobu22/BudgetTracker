using BudgetTracker.Core.Models;

namespace BudgetTracker.Core.Services.Interfaces
{
    public interface IImportQueueService
    {
        Task EnqueueImportAsync(int importId);
        Task<string?> DequeueImportAsync(int importId);

    }
}
