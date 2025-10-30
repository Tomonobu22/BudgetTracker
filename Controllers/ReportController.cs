using BudgetTracker.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BudgetTracker.Controllers
{
    public class ReportController : Controller
    {
        private readonly IReportAppService _reportAppService;
        public ReportController(IReportAppService reportAppService)
        {
            _reportAppService = reportAppService;
        }

        public async Task<IActionResult> Index()
        {
            var year = DateTime.Now.Year;
            var month = DateTime.Now.Month;
            var userId = "1"; // Replace with actual user ID retrieval logic
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            var model = await _reportAppService.GetSummaryReportAsync(userId, startDate, endDate);
            return View(model);
        }
    }
}
