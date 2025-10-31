using BudgetTracker.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BudgetTracker.Controllers
{
    [Authorize]
    public class ReportController : Controller
    {
        private readonly IReportAppService _reportAppService;
        public ReportController(IReportAppService reportAppService)
        {
            _reportAppService = reportAppService;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
