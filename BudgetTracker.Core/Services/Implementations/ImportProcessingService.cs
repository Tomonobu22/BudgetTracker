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
        private readonly IIncomeRepository _incomeRepository;
        private readonly IInvestmentRepository _investmentRepository;

        public ImportProcessingService(IImportRepository importRepository, IBlobStorageService blobStorageService, ICsvImportService csvImportService, IExpenseRepository expenseRepository, IIncomeRepository incomeRepository, IInvestmentRepository investmentRepository)
        {
            _importRepository = importRepository;
            _blobStorageService = blobStorageService;
            _csvImportService = csvImportService;
            _expenseRepository = expenseRepository;
            _incomeRepository = incomeRepository;
            _investmentRepository = investmentRepository;
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

                switch (import.ImportType)
                {
                    case RecordType.Expense:
                        var expenses = await _csvImportService.ParseExpenseAsync(stream, import.UserId, cancellationToken);
                        await _expenseRepository.AddRangeAsync(expenses);
                        break;
                    case RecordType.Income:
                        var incomes = await _csvImportService.ParseIncomeAsync(stream, import.UserId, cancellationToken);
                        await _incomeRepository.AddRangeAsync(incomes);
                        break;
                    case RecordType.Investment:
                        var investments = await _csvImportService.ParseInvestmentAsync(stream, import.UserId, cancellationToken);
                        await _investmentRepository.AddRangeAsync(investments);
                        break;
                    default:
                        throw new NotSupportedException($"File type {import.ImportType} is not supported.");
                }


                import.Status = ImportStatus.Completed;
                import.ProcessedAt = DateTime.UtcNow;
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
