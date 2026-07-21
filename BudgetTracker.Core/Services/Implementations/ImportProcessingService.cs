using BudgetTracker.Core.Enums;
using BudgetTracker.Core.Repositories.Interfaces;
using BudgetTracker.Core.Services.Interfaces;

namespace BudgetTracker.Core.Services.Implementations
{
    public class ImportProcessingService : IImportProcessingService
    {
        private readonly IImportRepository _importRepository;
        private readonly IBlobStorageService _blobStorageService;
        private readonly ICsvImportService _csvImportService;
        private readonly IExpenseRepository _expenseRepository;

        public ImportProcessingService(IImportRepository importRepository, IBlobStorageService blobStorageService, ICsvImportService csvImportService, IExpenseRepository expenseRepository)
        {
            _importRepository = importRepository;
            _blobStorageService = blobStorageService;
            _csvImportService = csvImportService;
            _expenseRepository = expenseRepository;
        }

        public async Task ProcessImportAsync(int importId, CancellationToken cancellationToken = default)
        {
            var import = await _importRepository.GetByIdAsync(importId);
            if (import == null) {
                throw new ArgumentException($"Import with ID {importId} not found.");
            }
            import.Status = ImportStatus.InProgress;
            await _importRepository.UpdateAsync(import);

            try { 
                var stream = await _blobStorageService.DownloadAsync(import.BlobName, cancellationToken);
                var expenses = await _csvImportService.ParseAsync(stream, import.UserId, cancellationToken);
                await _expenseRepository.AddRangeAsync(expenses);
                import.Status = ImportStatus.Completed;
                await _importRepository.UpdateAsync(import);
            } 
            catch (Exception ex)
            {
                import.Status = ImportStatus.Failed;
                import.ErrorMessage = ex.Message;
                await _importRepository.UpdateAsync(import);
                throw new Exception($"Failed to process import with ID {importId}.", ex);
            }
        }
    }
}
