using BudgetTracker.Models;

namespace BudgetTracker.Services.Interfaces
{
    public interface IReportAppService
    {
        Task<ReportSummaryViewModel> GetSummaryReportAsync (string userId, DateTime startDate, DateTime endDate);
    }
}
