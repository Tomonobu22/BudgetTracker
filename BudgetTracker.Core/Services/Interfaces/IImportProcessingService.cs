namespace BudgetTracker.Core.Services.Interfaces
{
    public interface IImportProcessingService
    {
        Task ProcessImportAsync(int importId, CancellationToken cancellationToken = default);
    }
}
