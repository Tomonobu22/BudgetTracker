using BudgetTracker.Models;

namespace BudgetTracker.Services.Interfaces
{
    public interface IImportQueueService
    {
        Task EnqueueImportAsync(int importId);
        Task<string?> DequeueImportAsync(int importId);

    }
}
