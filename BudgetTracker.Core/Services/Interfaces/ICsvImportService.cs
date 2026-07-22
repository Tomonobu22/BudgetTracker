using BudgetTracker.Core.DTOs;
using BudgetTracker.Core.Models;

namespace BudgetTracker.Core.Services.Interfaces
{
    public interface ICsvImportService
    {
        Task<IEnumerable<Income>> ParseIncomeAsync(Stream csvStream, string userId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Expense>> ParseExpenseAsync(Stream csvStream, string userId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Investment>> ParseInvestmentAsync(Stream csvStream, string userId, CancellationToken cancellationToken = default);
    }
}
