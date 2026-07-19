namespace BudgetTracker.Core.Services.Interfaces
{
    public interface IBlobStorageService
    {
        Task UploadAsync(Stream fileStream, string blobName, string contentType, CancellationToken cancellationToken = default);
    }
}
