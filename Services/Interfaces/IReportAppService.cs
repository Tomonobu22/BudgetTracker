using BudgetTracker.Models;

namespace BudgetTracker.Services.Interfaces
{
    public interface IReportAppService
    {
        Task<ReportSummaryViewModel> GetSummaryReportAsync (int userId, DateTime startDate, DateTime endDate);
    }
}
