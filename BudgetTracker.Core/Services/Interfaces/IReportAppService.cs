using BudgetTracker.Core.Models;

namespace BudgetTracker.Core.Services.Interfaces
{
    public interface IReportAppService
    {
        Task<ReportSummaryViewModel> GetSummaryReportAsync (string userId, DateTime startDate, DateTime endDate);
        Task<MonthlySummaryViewModel> GetMonthlySummaryAsync(string userId, int year);
        Task<List<int>> GetAvailableYearsAsync(string userId);
    }
}
